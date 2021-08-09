using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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

                // We create the world...
                m_objectFactory = new ObjectFactory();
                m_objectFactory.addRootFolder("../WorldLib/BuiltInObjects");
                m_worldManager = new WorldManager(m_objectFactory, "test-cave-1K");

                // We create the player and observe updates for them...
                m_player = m_worldManager.createPlayer("Dugong");
                m_player.onUpdate += onPlayerUpdate;

                // We look at the player's initial location...
                m_player.look();
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
                m_player.onUpdate -= onPlayerUpdate;

                // We unhook the logger...
                Logger.onMessageLogged -= onMessageLogged;
            }
            catch (Exception ex)
            {
                Logger.log(ex);
            }
        }

        private void ctrlInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ctrlOutput.AppendText($"> {ctrlInput.Text}{Environment.NewLine}");
                ctrlInput.Text = "";
                e.Handled = true;
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
                ctrlOutput.AppendText($"LOG: {args.Message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                ctrlOutput.AppendText($"LOG: {ex.Message}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Called when we see an update to a player or to what the player can see.
        /// </summary>
        private void onPlayerUpdate(object sender, Player.Args args)
        {
            try
            {
                foreach(var line in args.Text)
                {
                    ctrlOutput.AppendText($"{line}{Environment.NewLine}");
                }
            }
            catch(Exception ex)
            {
                Logger.log(ex);
            }
        }

        #endregion

        #region Private data

        // The object factory...
        private ObjectFactory m_objectFactory = null;

        // The world-manager...
        private WorldManager m_worldManager = null;

        // The player...
        private Player m_player = null;

        #endregion
    }
}
