using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Text.RegularExpressions;

namespace EVEMon
{
    /// <summary>
    /// Configuration editor form for the ToolTip style tray icon popup
    /// </summary>
    public partial class TrayTooltipConfigForm : EVEMonForm
    {
        private string m_tooltipString;

        // Array containing the example tooltip formats that are populated into the dropdown box.
        private string[] tooltipCodes = {
            "%n - %s %tr - %r",
            "%n - %s [%cr->%tr]: %r",
            "%n : %s - %d : %b isk",
            "%s %ci to %ti, %r left"
        };

        public string TooltipString
        {
            get { return m_tooltipString; }
            set { m_tooltipString = value; }
        }

        public TrayTooltipConfigForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            for (int i = 0; i < tooltipCodes.Length; i++)
            {
                cbTooltipDisplay.Items.Add(FormatExampleTooltipText(tooltipCodes[i]));
            }
            cbTooltipDisplay.Items.Add(" -- Custom -- ");

            tbTooltipString.Text = m_tooltipString;
            tbTooltipString_TextChanged(null, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_tooltipString = tbTooltipString.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tbTooltipString_TextChanged(object sender, EventArgs e)
        {
            tbTooltipTestDisplay.Text = FormatExampleTooltipText(tbTooltipString.Text);

            if (cbTooltipDisplay.SelectedIndex == -1)
            {
                int index = tooltipCodes.Length;

                for (int i = 0; i < tooltipCodes.Length; i++)
                {
                    if (tooltipCodes[i].Equals(tbTooltipString.Text))
                    {
                        index = i;
                    }
                }

                cbTooltipDisplay.SelectedIndex = index;
                DisplayCustomControls(index == tooltipCodes.Length);
            }
        }

        // Formats the argument format string with hardcoded exampe values.  Works basically the
        // same as MainWindow.FormatTooltipText(...), with the exception of the exampe values.
        private string FormatExampleTooltipText(string fmt)
        {
            return Regex.Replace(fmt, "%([nbsdr]|[ct][ir])", new MatchEvaluator(delegate(Match m)
                {
                    string value = String.Empty;
                    char capture = m.Groups[1].Value[0];

                    switch (capture)
                    {
                        case 'n':
                            value = "John Doe";
                            break;
                        case 'b':
                            value = "183,415,254.05";
                            break;
                        case 's':
                            value = "Gunnery";
                            break;
                        case 'd':
                            value = "9/15/2006 6:36 PM";
                            break;
                        case 'r':
                            value = "2h, 53m, 28s";
                            break;
                        default:
                            int level = -1;
                            if (capture == 'c')
                            {
                                level = 3;
                            }
                            else if (capture == 't')
                            {
                                level = 4;
                            }

                            if (m.Groups[1].Value.Length > 1 && level >= 0)
                            {
                                capture = m.Groups[1].Value[1];

                                if (capture == 'i')
                                {
                                    value = level.ToString();
                                }
                                else if (capture == 'r')
                                {
                                    value = Skill.GetRomanForInt(level);
                                }
                            }
                            break;
                    }

                    return value;
                }), RegexOptions.Compiled);
        }

        private void cbTooltipDisplay_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbTooltipDisplay.SelectedIndex;

            if (index == tooltipCodes.Length)
            {
                tbTooltipString.Text = Settings.GetInstance().TooltipString;
                DisplayCustomControls(true);
            }
            else
            {
                tbTooltipString.Text = tooltipCodes[index];
                DisplayCustomControls(false);
            }
        }

        /// <summary>
        /// Toggles the visibility of the tooltip example display and code label, as well as the readonly status of the tooltip string itself.
        /// </summary>
        /// <param name="custom">Show tbTooltipTestDisplay?</param>
        private void DisplayCustomControls(bool custom)
        {
            this.SuspendLayout();
            tbTooltipTestDisplay.Visible = custom;
            gbHelp.Visible = custom;
            tbTooltipString.ReadOnly = !custom;
            this.ResumeLayout();
        }

    }
}