namespace AmazonTracker.ProductScraper 

open System
open FSharp.Data

type ProductPage = 
    | Pantry of HtmlDocument
    | Default of HtmlDocument

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
    Price : Option<decimal>
    Title : string
    Description : string
    BulletPoints : seq<string>
  //  Reviews : seq<Review>
    BuyBoxSeller : Option<string>
    ThirdPartySeller : Option<string>
    ThirdPartyPrice : Option<decimal>
    LookUpDate : DateTime
}
