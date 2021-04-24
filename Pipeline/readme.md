# 🧪 Pipeline Solution

C# .NET 5 solution combinig all projects together to one pipeline.


### Installation Remarks
For the Web UI it is required to run a `netsh` configuration command:
```
netsh http add urlacl url=http://+:8080/ user=DOMAIN\USERNAME
```