using System;
using System.Collections.Generic;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    /////////////////////////////////////////////////////////////////////////////
    /////
    public class WhiteDefenderControl : FSMBossAreaControl
    {
        public override string FSMName => "Dung Defender";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Dolphin Max X" , x => edgeR},
                {"Dolphin Min X" , x => edgeL},
                {"Max X" , x => edgeR},
                {"Min X" , x => edgeL},
                {"Erupt Y" , x => floorY},
                {"Buried Y" , x => floorY - 3f},
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Will Evade?");
            this.AddResetToStateOnHide(control, "Init");
        }
    }



    public class WhiteDefenderSpawner : DefaultSpawner<WhiteDefenderControl> { }

    public class WhiteDefenderPrefabConfig : DefaultPrefabConfig<WhiteDefenderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
