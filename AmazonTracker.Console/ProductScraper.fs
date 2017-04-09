module ProductScraper

open System
open FSharp.Data
open System.Globalization

type PriceResult = {
     Price : decimal 
     ASIN : string
     LookUpDate: DateTime
}

type RawReview  = {
    Title: string
    Stars: string
    DateString: string
    Verified: bool
    Text: string
}

type Review = {
    Title: string
    Stars : double
    Date : DateTime
    Verified : bool
    Text : string
}

type Product = {
    ASIN : string
    Price : decimal
    Title : string
    Description : string
    BulletPoints : seq<string>
    Reviews : seq<Review>
    BuyBoxSeller : string
    LookUpDate : DateTime
}

let getAsinProductUrl asin =
    "https://amazon.com/exec/obidos/ASIN/" + asin

let loadHtmlDoc (url:string) : HtmlDocument = 
    HtmlDocument.Load(url)

let transformToReview (review : RawReview) = 
    {
        Title = review.Title
        Stars = Double.Parse (review.Stars.Substring(0, 2))
        Date = DateTime.Parse (review.DateString.Replace("on ", ""))
        Verified = review.Verified
        Text = review.Text
    }
    

let transformToRawReview (node:HtmlNode) =
    let starsAnchorTitle = node.CssSelect ".a-link-normal" |> Seq.head |> (fun node -> node.Attribute "title") |> (fun a -> a.Value())
    let title = node.CssSelect ".review-title" |> Seq.head |> (fun node -> node.InnerText())
    let date = node.CssSelect "span.review-date" |> Seq.head |> (fun node -> node.InnerText())
    let verified = node.Descendants (fun node -> node.AttributeValue "data-hook" = "avp-badge") |> Seq.isEmpty = false
    let text = node.CssSelect ".review-text" |> Seq.head |> (fun node -> node.InnerText())
    
    {
     Title = title
     Stars = starsAnchorTitle
     DateString = date
     Verified = verified
     Text = text
    }

let rec accumulateReviews reviews (link:string) = 
    let reviewDoc = HtmlDocument.Load(link)
    let currentReviews = reviewDoc.CssSelect ".a-section.review" 
                             |> Seq.map (fun node -> transformToRawReview node)  
                             |> Seq.map (fun review -> transformToReview review)
   
    let appendedReviews = Seq.append reviews currentReviews 
  
    let nextLinkNode = reviewDoc.CssSelect "#cm_cr-pagination_bar .a-last a" |> Seq.tryHead
   
    let nextLink = match nextLinkNode with
        | Some l -> Some (("https://amazon.com" + (l.AttributeValue "href")))
        | None -> None

    match nextLink with 
        | Some l -> accumulateReviews appendedReviews l
        | None _ -> appendedReviews 

let getReviews (productHtmlDoc : HtmlDocument) = 
    let reviewSummaryNode = productHtmlDoc.CssSelect "#reviewSummary .a-row a" |> Seq.head
    let allReviewsLink = reviewSummaryNode.AttributeValue "href" |> (fun s -> "https://amazon.com" + s)
    let initReviews = Seq.empty<Review>
    accumulateReviews initReviews allReviewsLink

let getPrice (productPage : HtmlDocument) = 
    let price = productPage.CssSelect("#priceblock_ourprice")
                |> Seq.head
                |> (fun node -> node.InnerText())
                |> (fun innerText -> Decimal.Parse(innerText, NumberStyles.AllowCurrencySymbol ||| NumberStyles.Currency ||| NumberStyles.Number))

    price

let getTitle (productHtmlDoc : HtmlDocument) = 
    productHtmlDoc.CssSelect "#productTitle" |> Seq.head |> (fun node -> node.InnerText())

let getProductDescription (productHtmlDoc : HtmlDocument) =
    let desc = productHtmlDoc.CssSelect "#productDescription"
                |> Seq.head
                |> (fun desc -> desc.CssSelect "p")
                |> Seq.map (fun i -> i.InnerText())
                |> Seq.fold (fun r s -> r + " \n " + s) ""

    desc

let getProductSummaryPoints (productHtmlDoc : HtmlDocument) = 
    productHtmlDoc.CssSelect "#feature-bullets ul li .a-list-item" 
    |> Seq.map (fun node -> node.InnerText()) 
//
//let getSoldby (productHtmlDoc : HtmlDocument) = 
//    let soldByThirdPartyNode = productHtmlDoc.CssSelect "#soldByThirdParty" |> Seq.tryHead
//
//    match soldByThirdPartyNode with 
//        | Some n -> n.

let getSoldByAvail (productHtmlDoc: HtmlDocument) = 
    let soldBy = productHtmlDoc.CssSelect "#availability-brief #merchant-info" 
                                |> Seq.head 
                                |> (fun node -> node.Descendants "a") 
                                |> Seq.head 
                                |> (fun node -> node.InnerText())


    soldBy

let getProductInfo asin =
    let productPageUrl = getAsinProductUrl asin
    let productHtmlDoc : HtmlDocument = loadHtmlDoc productPageUrl

    let title = getTitle productHtmlDoc
    let price = getPrice productHtmlDoc
    let productDesc = getProductDescription productHtmlDoc
    let featurePoints = getProductSummaryPoints productHtmlDoc
    let reviews = getReviews productHtmlDoc
    let soldBy = getSoldByAvail productHtmlDoc
    {
        ASIN = asin
        Title = title
        Description = productDesc
        Price = price
        BulletPoints = featurePoints
        Reviews = reviews
        LookUpDate = DateTime.UtcNow
        BuyBoxSeller = soldBy
    }



