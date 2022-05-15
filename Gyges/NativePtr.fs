namespace Gyges

#nowarn "9"

open FSharp.NativeInterop

module NativePtr =
    let inline isNotNullPtr (address: nativeptr<'T>) = not (NativePtr.isNullPtr address)


