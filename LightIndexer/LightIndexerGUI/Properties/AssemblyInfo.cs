using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LightIndexer GUI")]
[assembly: AssemblyDescription("Indexing application based on Lucene.NET Apache project v3.0.3.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("LightIndexer")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0c260fa9-d651-40a2-abc9-af493ff07e28")]


// Configure log4net using the .log4net file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]