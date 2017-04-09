// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open FSharp.Data
open System
open System.Globalization
open System.Threading
open System.IO
open Microsoft.FSharp.Collections
open Newtonsoft.Json

open ProductScraper
//type PriceResult = 
//    {Price : decimal 
//     ASIN : string
//     LookUpDate: DateTime}
//
//type RawReview  = {
//    Title: string
//    Stars: string
//    DateString: string
//    Verified: bool
//    Text: string
//}
//
//type Review = {
//    Title: string
//    Stars : string
//    Date : DateTime
//    Verified : bool
//    Text : string
//}
//
//type Product = {
//    ASIN : string
//    Price : decimal
//    Title : string
//    Decription : string
//    BulletPoints : seq<string>
//    Reviews : seq<Review>
//    LookUpDate : DateTime
//}
//
//let loadHtmlDoc (url:string) : HtmlDocument = 
//    HtmlDocument.Load(url)
//
//let transformToRawReview (node:HtmlNode) =
//    let starsAnchorTitle = node.CssSelect ".a-link-normal" |> Seq.head |> (fun node -> node.Attribute "title") |> (fun a -> a.Value())
//    let title = node.CssSelect ".review-title" |> Seq.head |> (fun node -> node.InnerText())
//    let date = node.CssSelect "span.review-date" |> Seq.head |> (fun node -> node.InnerText())
//    let verified = node.Descendants (fun node -> node.AttributeValue "data-hook" = "avp-badge") |> Seq.isEmpty = false
//    let text = node.CssSelect ".review-text" |> Seq.head |> (fun node -> node.InnerText())
//    
//    {
//     Title = title
//     Stars = starsAnchorTitle
//     DateString = date
//     Verified = verified
//     Text = text
//    }
//
////let transformToReview raw = 
//    
//
//let rec getReviews reviews (link:string) = 
//    printfn "%A" link
//    let reviewDoc = HtmlDocument.Load(link)
//    let currentReviews = reviewDoc.CssSelect ".a-section.review" |> Seq.map transformToRawReview  |> Seq.map transformToReview
//    printfn "Reviews Found: %A" (Seq.length currentReviews)
//
//    let appendedReviews = Seq.append reviews currentReviews 
//   
//
//    let nextLinkNode = reviewDoc.CssSelect "#cm_cr-pagination_bar .a-last a" |> Seq.tryHead
//   
//    let nextLink = match nextLinkNode with
//        | Some l -> Some (("https://amazon.com" + (l.AttributeValue "href")))
//        | None -> None
//
//    match nextLink with 
//        | Some l -> getReviews appendedReviews l
//        | None _ -> appendedReviews 
//
//
//
//let getPrice htmlDocLoader asin = 
//    let productPageUrl = "https://amazon.com/exec/obidos/ASIN/" + asin
//    let productPage : HtmlDocument = htmlDocLoader productPageUrl
//
//    let price = productPage.CssSelect("#priceblock_ourprice")
//                |> Seq.head
//                |> (fun node -> node.InnerText())
//                |> (fun innerText -> Decimal.Parse(innerText, NumberStyles.AllowCurrencySymbol ||| NumberStyles.Currency ||| NumberStyles.Number))
//
//    { Price = price 
//      ASIN = asin 
//      LookUpDate = DateTime.UtcNow }
//
//let appendText path text =
//     File.AppendAllText(path, text + "\n")

[<EntryPoint>]
let main argv =

    let asin = argv.[0]

    let product = getProductInfo asin

    printfn "%A" product

    let json = JsonConvert.SerializeObject product

    File.WriteAllText((@"C:\temp\" + asin + ".json"), json) 
    0
