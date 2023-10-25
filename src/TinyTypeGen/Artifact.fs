module TinyTypGen.Artifact

open System
open TinyTypeGen

type CacheInfo =
  {
    Name: string
    HashValue: int
  }

type Artifact =
  {
    FileName: string
    Hash: int
    Path: string
    Content: string
  }

let getCacheInfo path =
  let path = IO.Path.Combine(path, "cacheinfo")
  if IO.File.Exists path then
    path
    |> IO.File.ReadAllLines
    |> Seq.map _.Split("=")
    |> Seq.map (fun x ->
      {
        Name = x[0]
        HashValue = x[1] |> int
      }
    )

  else
    []

let getCachedCacheInfo = Utils.memoize getCacheInfo

let getStableHashCode (str: string) =
  let mutable hash1 = (5381 <<< 16) + 5381
  let mutable hash2 = hash1

  for i in 0..2 .. (str.Length - 1) do
    hash1 <- ((hash1 <<< 5) + hash1) ^^^ (int str.[i])

    if i < str.Length - 1 then
      hash2 <- ((hash2 <<< 5) + hash2) ^^^ (int str.[i + 1])

  hash1 + (hash2 * 1566083941)

let toArtifact fileName _ content path =
  content
  |> getStableHashCode
  |> fun hash ->
      {
        Artifact.FileName = fileName
        Hash = hash
        Path = path
        Content = content
      }
