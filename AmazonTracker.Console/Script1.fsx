
#r@"C:\projects\AmazonTracker\packages\FSharp.Data.2.3.2\lib\net40\FSharp.Data.dll"

open FSharp.Data

type AmazonProductPage = HtmlProvider<"https://www.amazon.com/exec/obidos/asin/B000ILIHA6">

let blueDog = AmazonProductPage.Load("https://www.amazon.com/exec/obidos/asin/B000ILIHA6")

blueDog.Lists.``Product details``

let prodDetails = blueDog.Lists.``Product details``
let list1 = blueDog.Lists.``List2``