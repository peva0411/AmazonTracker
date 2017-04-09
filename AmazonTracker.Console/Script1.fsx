
#r@"C:\projects\AmazonTracker\packages\FSharp.Data.2.3.2\lib\net40\FSharp.Data.dll"

open FSharp.Data

type AmazonProductPage = HtmlProvider<"https://www.amazon.com/exec/obidos/asin/B000ILIHA6">

let blueDog = AmazonProductPage.Load("https://www.amazon.com/exec/obidos/asin/B000ILIHA6")

blueDog.Lists.``Product details``

let prodDetails = blueDog.Lists.``Product details``

let document = HtmlDocument.Load("https://www.amazon.com/exec/obidos/asin/B000ILIHA6")

let title = document.CssSelect "#productTitle" |> Seq.head

let prodDesc = document.CssSelect "#productDescription"

let prodItems = prodDesc.CssSelect "p"

let descrips  = prodItems |> Seq.map (fun node -> node.InnerText())

let featurePointsText = document.CssSelect "#feature-bullets ul li .a-list-item" |> Seq.map (fun node -> node.InnerText()) |> printfn "%A" 


let reviewSummaryNode = document.CssSelect "#reviewSummary .a-row a" |> Seq.head

let allReviewsLink = reviewSummaryNode.AttributeValue "href" |> (fun s -> "https://amazon.com" + s)

let allReviews = HtmlDocument.Load(allReviewsLink)

type Review  = {
    Title: string
    Stars: string
    Date: string
    Verified: bool
    Text: string
}

let transformToReview (node:HtmlNode) =
    let starsAnchorTitle = node.CssSelect ".a-link-normal" |> Seq.head |> (fun node -> node.Attribute "title") |> (fun a -> a.Value())
    let title = node.CssSelect ".review-title" |> Seq.head |> (fun node -> node.InnerText())
    let date = node.CssSelect "span.review-date" |> Seq.head |> (fun node -> node.InnerText())
    let verified = node.Descendants (fun node -> node.AttributeValue "data-hook" = "avp-badge") |> Seq.isEmpty = false
    let text = node.CssSelect ".review-text" |> Seq.head |> (fun node -> node.InnerText())
    
    {Title = title
     Stars = starsAnchorTitle
     Date = date
     Verified = verified
     Text = text
    }


let rec getReviews reviews (link:string) = 
    printfn "%A" link
    let reviewDoc = HtmlDocument.Load(link)
    let currentReviews = reviewDoc.CssSelect ".a-section.review" |> Seq.map transformToReview |> Seq.append reviews 
    printfn "%A" currentReviews

    let nextLinkNode = reviewDoc.CssSelect "#cm_cr-pagination_bar .a-last" |> Seq.tryHead
   
    let nextLink = match nextLinkNode with
        | Some l -> Some (l.AttributeValue "href")
        | None -> None

    match nextLink with 
        | Some l -> printfn "%A" l
        | None _ -> printfn "No Value"

    match nextLink with 
        | Some l -> getReviews currentReviews l
        | None _ -> currentReviews 

