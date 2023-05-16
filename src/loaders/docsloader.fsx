#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "../../.paket/load/net5.0/main.group.fsx"

open System.IO
open Fornax.Nfdi4Plants

let contentDir = "docs"

type DocsConfig = {
    disableLiveRefresh: bool
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    let docsPath = Path.Combine(projectRoot, contentDir)
    // let options = EnumerationOptions(RecurseSubdirectories = true)
    // let files = Directory.GetFiles(docsPath, "*"), options)
    let files = 
        Directory.GetFiles(docsPath, "*")
        |> Array.filter (fun n -> n.Contains @"\_sidebars\" |> not && n.Contains "/_sidebars/" |> not)
        |> Array.filter (fun n -> n.Contains @"\_ignored\" |> not && n.Contains "/_ignored/" |> not)
        |> Array.filter (fun n -> n.EndsWith ".md")
        |> Array.filter (fun n -> n.Contains "README.md" |> not)
    
    let docs = 
        let loadDocs (filePath:string) = 
            #if WATCH
            Docs.loadFile(projectRoot, contentDir, filePath, useNewSidebar = true, includeSearchbar = true)
            #else
            Docs.loadFile(projectRoot, contentDir, filePath, useNewSidebar = true, includeSearchbar = true, productionBasePath = "web-components-docs")
            #endif
        files 
        |> Array.map loadDocs

    docs
    |> Array.iter(fun doc -> printfn "[SIDEBAR %s] %i" doc.title doc.sidebar.Length)    

    // let docs0 = siteContent.TryGetValues<DocsData> () |> Option.defaultValue Seq.empty

    // printfn "LOADER: %A" <| Seq.length docs0

    docs
    |> Array.iter siteContent.Add

    printfn "[DOCS-LOADER] Done!"

    siteContent.Add({disableLiveRefresh = false})
    siteContent
