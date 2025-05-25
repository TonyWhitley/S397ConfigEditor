using System;
using System.Reflection;
using System.Windows.Forms;

namespace S397ConfigEditor;

partial class AboutBox1 : Form
{
    internal AboutBox1()
    {
        InitializeComponent();
        this.Text = String.Format("About {0}", AssemblyTitle);
        this.labelProductName.Text = AssemblyProduct;
        this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
        this.labelCopyright.Text = AssemblyCopyright;
        this.labelCompanyName.Text = AssemblyCompany;
        this.textBoxDescription.Text = AssemblyDescription +
                                       Environment.NewLine +
                                       Environment.NewLine +
                                       "Can also run from the command line.";
    }

    #region Assembly Attribute Accessors

    private string AssemblyTitle
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != "")
                {
                    return titleAttribute.Title;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }
    }

    private string AssemblyVersion
    {
        get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    private string AssemblyDescription
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }
    }

    private string AssemblyProduct
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }

    private string AssemblyCopyright
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    private string AssemblyCompany
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
    }
    #endregion
}
