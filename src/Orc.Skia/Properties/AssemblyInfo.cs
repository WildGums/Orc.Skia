// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Reflection;
using System.Resources;

#if NET || NETCORE
using System.Windows;
using System.Windows.Markup;
#endif

// All other assembly info is defined in SharedAssembly.cs

[assembly: AssemblyTitle("Orc.Skia")]
[assembly: AssemblyProduct("Orc.Skia")]
[assembly: AssemblyDescription("Orc.Skia library")]
[assembly: NeutralResourcesLanguage("en-US")]

#if NET || NETCORE
[assembly: XmlnsPrefix("http://schemas.wildgums.com/orc/skia", "orcskia")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Behaviors")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Controls")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Converters")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Fonts")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Markup")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Views")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/skia", "Orc.Skia.Windows")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
#endif
