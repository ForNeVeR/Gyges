module Gyges.Utils

open System

module Map =
    let mapValues (mapper: 'a -> 'b) (table: Map<_, 'a>): Map<_, 'b> =
        Map.map (fun _ v -> mapper v) table
        
    let filterValues (predicate: 'a -> bool) (table: Map<_, 'a>): Map<_, 'a> =
        Map.filter (fun _ v -> predicate v) table
        
module GuidMap =
    let add (value: 'a) (table: Map<Guid, 'a>): Map<Guid, 'a> =
        Map.add (Guid.NewGuid()) value table