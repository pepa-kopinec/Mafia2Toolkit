﻿using System.Windows.Forms;
using Utils.Lang;
using ResourceTypes.Actors;

namespace Forms.EditorControls
{
    public partial class ActorItemAddOption : UserControl
    {
        public ActorItemAddOption()
        {
            InitializeComponent();
            Localise();

            TypeCombo.Items.Add(ActorTypes.C_TrafficCar);
            TypeCombo.Items.Add(ActorTypes.C_TrafficHuman);
            TypeCombo.Items.Add(ActorTypes.C_TrafficTrain);
            TypeCombo.Items.Add(ActorTypes.C_Item);
            TypeCombo.Items.Add(ActorTypes.C_Door);
            TypeCombo.Items.Add(ActorTypes.C_Sound);
            TypeCombo.Items.Add(ActorTypes.StaticEntity);
            TypeCombo.Items.Add(ActorTypes.FrameWrapper);
            TypeCombo.Items.Add(ActorTypes.C_ActorDetector);
            TypeCombo.Items.Add(ActorTypes.LightEntity);
            TypeCombo.Items.Add(ActorTypes.C_ScriptEntity);
            TypeCombo.Items.Add(ActorTypes.C_Pinup);

            TypeCombo.SelectedIndex = 0;
        }

        private void Localise()
        {
            groupGeneral.Text = Language.GetString("$GENERAL");
            ActorTypeLabel.Text = Language.GetString("$ACTOR_TYPE");
        }

        public ActorTypes GetSelectedType()
        {
            return (ActorTypes)TypeCombo.SelectedItem;
        }
    }
}
