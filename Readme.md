<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128585716/13.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E5139)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Program.cs](./CS/XPOConsoleApplication/Program.cs) (VB: [Program.vb](./VB/XPOConsoleApplication/Program.vb))
<!-- default file list end -->
# How to create collection properties with associations at runtime


<p>The XPO Library provides the capability to create classes and members at runtime by customizing the metadata dictionary. Besides creating persistent members mapped to database columns, it is also possible to create collection-type members with associations. This example demonstrates both one-to-many and many-to-many associations added to existing persistent classes dynamically and how dynamic properties can be accessed.</p><p>Note that all metadata customizations must be done before a data layer is created with the customized dictionary.</p><p>See also:<br />
XAF-specifics: <a href="https://www.devexpress.com/Support/Center/p/E250">How to customize a Business Model at runtime (Example).</a><u><br />
</u>Related XPO classes: <a href="https://documentation.devexpress.com/#XPO/DevExpressXpoMetadata"><u>DevExpress.Xpo.Metadata Namespace</u></a></p>

<br/>


