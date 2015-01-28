using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string alert = "test ' \" \n \t aaa c:\\win";
        alert = CleanJSString(alert);

        string script = "alert(\"" + alert + "\")";
        ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
    }

    private static readonly IDictionary<string, string> replacements = new Dictionary<string, string>
                                                                          {
                                                                              {"\\","\\\\"},
                                                                              {"'","\\'"},
                                                                              {"\"","\\\""},
                                                                              {"\n","\\\n"},
                                                                              {"\t","\\\t"},
                                                                          };

    public static string CleanJSString(string jsString)
    {
        if (string.IsNullOrEmpty(jsString))
        {
            return jsString;
        }

        StringBuilder sb = new StringBuilder(jsString);

        foreach (KeyValuePair<string, string> replacement in replacements)
        {
            sb.Replace(replacement.Key, replacement.Value);
        }

        return sb.ToString();
    }
}
