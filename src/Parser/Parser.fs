namespace Parser
#if INTERACTIVE
#r "../../packages/FSharp.Data/lib/netstandard2.0/FSharp.Data.dll"
#endif

open System
open FSharp.Data

module Parser =
    let getDt (address: string) =         
        let unix =
            DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc)    
        let ebay = HtmlDocument.Load address
        let body = ebay.Body()
        let rec parse (node: HtmlNode) =
            let exists =
                node.Elements()
                |> Seq.exists (fun node -> node.HasClass "timeMs")
            if not exists then
                node.Elements()
                |> Seq.collect (parse)
            else seq {
                yield (node.Elements()
                       |> Seq.tryFind (fun node -> node.HasClass "timeMs"))
            }    
        let node2dt =
            HtmlNode.attribute "timems"
            >> HtmlAttribute.value
            >> Double.Parse
            >> unix.AddMilliseconds
    
        parse body
        |> Seq.tryFind Option.isSome
        |> Option.map Option.get
        |> Option.map node2dt