// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open FSharp.Data
open System
open System.Globalization
open System.Threading
open System.IO

type PriceResult = 
    {Price : decimal 
     ASIN : string
     LookUpDate: DateTime}

let loadHtmlDoc (url:string) : HtmlDocument = 
    HtmlDocument.Load(url)

let getPrice htmlDocLoader asin = 
    let productPageUrl = "https://amazon.com/exec/obidos/ASIN/" + asin
    let productPage : HtmlDocument = htmlDocLoader productPageUrl

    let price = productPage.CssSelect("#priceblock_ourprice")
                |> Seq.head
                |> (fun node -> node.InnerText())
                |> (fun innerText -> Decimal.Parse(innerText, NumberStyles.AllowCurrencySymbol ||| NumberStyles.Currency ||| NumberStyles.Number))

    { Price = price 
      ASIN = asin 
      LookUpDate = DateTime.UtcNow }

let appendText path text =
     File.AppendAllText(path, text + "\n")

[<EntryPoint>]
let main argv =
    let intervalMinutes = float argv.[1]
    let asin = argv.[0]
    let path = argv.[2]

    let toOutputFile = appendText path
    toOutputFile "Date,ASIN,Price"

    let formatPriceResult priceResult= 
        sprintf "%s,%s,%f" (priceResult.LookUpDate.ToString()) priceResult.ASIN priceResult.Price

    while true do 
        getPrice loadHtmlDoc asin 
            |> formatPriceResult
            |> (fun formatedResult -> 
                printfn "%s" formatedResult 
                formatedResult)
            |> toOutputFile
        Thread.Sleep (TimeSpan.FromMinutes intervalMinutes)
 
    0
