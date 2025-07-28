# Keys

## Assembly Key

Place `Assembly.snk` file in this folder to sign produced .NET assemblies with the specified 
assembly key. If the key file is not present, the built assemblies are not signed.

Alternatively, you can set `AssemblyOriginatorKeyFile` environment variable to the path of
the desired assembly key file.
