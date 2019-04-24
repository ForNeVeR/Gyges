module Gyges.Utils

module Map =
    let mapValues (mapper: 'a -> 'b) (table: Map<_, 'a>): Map<_, 'b> =
        Map.map (fun _ v -> mapper v) table
        
    let filterValues (predicate: 'a -> bool) (table: Map<_, 'a>): Map<_, 'a> =
        Map.filter (fun _ v -> predicate v) table