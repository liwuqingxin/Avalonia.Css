// using System.Diagnostics;
//
// namespace Nlnet.Avalonia.Css.Test
// {
//     [Obsolete]
//     [TestClass]
//     public class ObsoletesTests
//     {
//         [Obsolete]
//         [TestMethod]
//         public void EfficientCssParserTest()
//         {
//             IAcssBuilder builder  = new AcssBuilder();
//
//             var parser   = builder.Parser;
//             var cssFile  = File.ReadAllText("./Assets/nlnet.blog.css");
//             var sections = parser.ParseSections(null, cssFile);
//             var styles   = sections.OfType<IAcssStyle>();
//
//             foreach (var acssStyle in styles)
//             {
//                 Trace.WriteLine(acssStyle.ToString());
//             }
//         }
//
//         [Obsolete]
//         [TestMethod]
//         public void TypeProviderTest()
//         {
//             IAcssBuilder builder  = new AcssBuilder();
//
//             var acssFile  = File.ReadAllText("./Assets/avalonia.controls.css");
//             var parser   = builder.Parser;
//             var css      = parser.RemoveComments(new Span<char>(acssFile.ToCharArray()));
//             var sections = parser.ParseSections(null, css);
//             var styles   = sections.OfType<IAcssStyle>();
//
//             foreach (var acssStyle in styles)
//             {
//                 var style    = acssStyle.ToAvaloniaStyle();
//                 var selector = style!.Selector;
//                 Trace.WriteLine(selector != null ? selector.ToString() : "<selector is null.>");
//             }
//         }
//     }
// }