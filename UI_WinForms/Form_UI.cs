using System;
using System.IO;
using System.Windows.Forms;
using Utility;
using WorldLib;

namespace UI_WinForms
{
    /// <summary>
    /// UI for playing a single-player adventure.
    /// </summary>
    public partial class Form_UI : Form
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Form_UI()
        {
            InitializeComponent();
        }

        #endregion

        #region Form events

        /// <summary>
        /// Called when the form has been loaded.
        /// </summary>
        private void Form_UI_Load(object sender, EventArgs e)
        {
            try
            {
                // We hook up to the logger...
                Logger.onMessageLogged += onMessageLogged;

                // We load the config...
                var jsonConfig = File.ReadAllText("config.json");
                var config = Utils.fromJSON<ParsedConfig>(jsonConfig);

                // We create the world, and load object definitions...
                m_worldManager = new WorldManager(config.StartingLocationID);
                foreach(var folder in config.ObjectFolders)
                {
                    m_worldManager.ObjectFactory.addRootFolder(folder);
                }
                m_worldManager.ObjectFactory.validateObjects();
                m_worldManager.resetWorld();

                // We create the player and observe updates for them...
                m_player = m_worldManager.createPlayer("Dugong");
                m_player.onUIUpdate += onPlayerUIUpdate;

                // We look at the player's initial location...
                m_player.look();

                // We start the timer which updates world state at regular intervals...
                m_worldUpdateTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called shortly before the form has closed.
        /// </summary>
        private void Form_UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // We stop observing the player...
                m_player.onUIUpdate -= onPlayerUIUpdate;

                // We unhook the logger...
                Logger.onMessageLogged -= onMessageLogged;
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called when a key is pressed in the input box.
        /// When Enter has been pressed we parse the input and perform the action.
        /// </summary>
        private void ctrlInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar != (char)Keys.Enter)
                {
                    return;
                }

                // Enter was pressed.
                // We echo the input to the output window...
                var input = ctrlInput.Text;
                ctrlOutput.AppendText($"> {input}{Environment.NewLine}");
                ctrlInput.Text = "";
                e.Handled = true;  // This stops the 'ding'

                // We store this input (to use again if up-arrow is pressed)...
                m_previousInput = input.Trim();

                // We pass the input to the player...
                m_player.parseInput(input);
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called shortly before key-down events.
        /// </summary>
        private void ctrlInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs args)
        {
            try
            {
                if (args.KeyCode == Keys.Up)
                {
                    // The up arrow is pressed, so we set the input text to the previous input.
                    // NOTE: We add a space to the end of the previous input, as (for some reason)
                    //       putting the cursor at the end seems to put it one place before the end.
                    ctrlInput.Text = $"{m_previousInput} ";
                    ctrlInput.SelectionStart = ctrlInput.TextLength;
                    ctrlInput.SelectionLength = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        /// <summary>
        /// Called when the world update timer ticks.
        /// </summary>
        private void m_worldUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                m_worldManager.update();
            }
            catch(Exception ex)
            {
                Logger.log(ex);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Called when a message has been logged.
        /// </summary>
        private void onMessageLogged(object sender, Logger.Args args)
        {
            try
            {
                ctrlOutput.AppendText($"{args.Message}{Environment.NewLine}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                ctrlOutput.AppendText($"{ex.Message}{Environment.NewLine}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Called when we see an update to a player or to what the player can see.
        /// </summary>
        private void onPlayerUIUpdate(object sender, Player.UIUpdateArgs args)
        {
            try
            {
                foreach(var line in args.Text)
                {
                    ctrlOutput.AppendText($"{line}{Environment.NewLine}");
                }
                ctrlOutput.AppendText($"{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        #endregion

        #region Private data

        // The world-manager...
        private WorldManager m_worldManager = null;

        // The player...
        private Player m_player = null;

        // The previous input...
        private string m_previousInput = "";

        #endregion
    }
}
