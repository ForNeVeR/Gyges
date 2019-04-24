module Gyges.Utils

open System

module Map =
    let mapValues (mapper: 'a -> 'b) (table: Map<_, 'a>): Map<_, 'b> =
        Map.map (fun _ v -> mapper v) table
        
    let filterValues (predicate: 'a -> bool) (table: Map<_, 'a>): Map<_, 'a> =
        Map.filter (fun _ v -> predicate v) table

    let addWithGuid (value: 'a) (table: Map<Guid, 'a>): Map<Guid, 'a> =
        Map.add (Guid.NewGuid()) value table
        
    let removeKeys keys (map : Map<'Key, 'T>) =
        if Set.isEmpty keys then
            map
        elif Map.isEmpty map then
            Map.empty
        else
            (map, keys)
            ||> Set.fold (fun map key ->
                Map.remove key map)