// ReSharper disable UnassignedField.Global
namespace Appalachia.Simulation.Trees.Icons
{
    public static class TreeIcons 
    {
        private static bool _initialized;

        public static TreeIcon addBranch;
        public static TreeIcon addBranchfrond;
        public static TreeIcon addDead;
        public static TreeIcon addDeadFelled;
        public static TreeIcon addDeadFelledRotted;
        public static TreeIcon addFelled;
        public static TreeIcon addFelledBare;
        public static TreeIcon addFelledBareRotted;
        public static TreeIcon addForest;
        public static TreeIcon addFrond;
        public static TreeIcon addFruit;
        public static TreeIcon addFungus;
        public static TreeIcon addKnot;
        public static TreeIcon addLeaf;
        public static TreeIcon addNewTree;
        public static TreeIcon addRoot;
        public static TreeIcon addStump;
        public static TreeIcon addStumpRotted;
        public static TreeIcon addTree;
        public static TreeIcon addTrunk;
        
        public static TreeIcon age10;
        public static TreeIcon age20;
        public static TreeIcon age35;
        public static TreeIcon age50;
        public static TreeIcon spirit;
        
        public static TreeIcon grab;
        public static TreeIcon move;
        public static TreeIcon rotate;
        public static TreeIcon scale;
        public static TreeIcon preview;
        public static TreeIcon preview2;
        public static TreeIcon preview3;
        public static TreeIcon color;
        public static TreeIcon save;
        public static TreeIcon wind;
        public static TreeIcon mesh;
        public static TreeIcon mesh2;
        public static TreeIcon refresh;
        public static TreeIcon label;
        public static TreeIcon ground;
        
        public static TreeIcon addAge10;
        public static TreeIcon addAge20;
        public static TreeIcon addAge35;
        public static TreeIcon addAge50;
        public static TreeIcon addSpirit;
        
        public static TreeIcon removeAge10;
        public static TreeIcon removeAge20;
        public static TreeIcon removeAge35;
        public static TreeIcon removeAge50;
        public static TreeIcon removeSpirit;
        
        public static TreeIcon disabledAge10;
        public static TreeIcon disabledAge20;
        public static TreeIcon disabledAge35;
        public static TreeIcon disabledAge50;
        public static TreeIcon disabledSpirit;

        public static TreeIcon branch;
        public static TreeIcon branch2;
        public static TreeIcon branchfrond;
        public static TreeIcon dead;
        public static TreeIcon deadFelled;
        public static TreeIcon deadFelledRotted;

        public static TreeIcon disabledBranch;
        public static TreeIcon disabledBranch2;
        public static TreeIcon disabledBranchfrond;
        public static TreeIcon disabledDead;
        public static TreeIcon disabledDeadFelled;
        public static TreeIcon disabledDeadFelledRotted;
        public static TreeIcon disabledFelled;
        public static TreeIcon disabledFelledBare;
        public static TreeIcon disabledFelledBareRotted;
        public static TreeIcon disabledForest;
        public static TreeIcon disabledFrond;
        public static TreeIcon disabledFruit;
        public static TreeIcon disabledFungus;
        public static TreeIcon disabledHammer;
        public static TreeIcon disabledKnot;
        public static TreeIcon disabledLeaf;
        public static TreeIcon disabledNewTree;
        public static TreeIcon disabledRoot;
        public static TreeIcon disabledStump;
        public static TreeIcon disabledStumpRotted;
        public static TreeIcon disabledTrash;
        public static TreeIcon disabledTree;
        public static TreeIcon disabledTrunk;

        public static TreeIcon disabledGrab;
        public static TreeIcon disabledMove;
        public static TreeIcon disabledRotate;
        public static TreeIcon disabledScale;
        public static TreeIcon disabledPreview;
        public static TreeIcon disabledPreview2;
        public static TreeIcon disabledPreview3;
        public static TreeIcon disabledColor;
        public static TreeIcon disabledSave;
        public static TreeIcon disabledWind;
        public static TreeIcon disabledMesh;
        public static TreeIcon disabledMesh2;
        public static TreeIcon disabledRefresh;
        public static TreeIcon disabledLabel;
        public static TreeIcon disabledGround;

        public static TreeIcon disabledLocked;
        public static TreeIcon disabledUnlocked;
        public static TreeIcon disabledRepair;
        public static TreeIcon disabledPaint;
        public static TreeIcon disabledImpostor;
        public static TreeIcon disabledCollision;
        public static TreeIcon disabledMagnifyingGlass;
        public static TreeIcon disabledBomb;
        public static TreeIcon disabledPlay;
        public static TreeIcon disabledPause;
        public static TreeIcon disabledCopy;
        public static TreeIcon disabledGear;
        public static TreeIcon disabledScene;
        public static TreeIcon disabledVisible;
        public static TreeIcon disabledX;
        
        public static TreeIcon felled;
        public static TreeIcon felledBare;
        public static TreeIcon felledBareRotted;
        public static TreeIcon forest;
        public static TreeIcon frond;
        public static TreeIcon fruit;
        public static TreeIcon fungus;
        public static TreeIcon hammer;
        public static TreeIcon knot;
        public static TreeIcon leaf;
        public static TreeIcon minus;
        public static TreeIcon newTree;
        public static TreeIcon plus;
        public static TreeIcon root;
        public static TreeIcon stump;
        public static TreeIcon stumpRotted;
        public static TreeIcon trash;
        public static TreeIcon tree;
        public static TreeIcon trunk;

        public static TreeIcon locked;
        public static TreeIcon unlocked;
        public static TreeIcon repair;
        public static TreeIcon paint;
        public static TreeIcon impostor;
        public static TreeIcon collision;
        public static TreeIcon magnifyingGlass;
        public static TreeIcon bomb;
        public static TreeIcon play;
        public static TreeIcon pause;
        public static TreeIcon copy;
        public static TreeIcon gear;
        public static TreeIcon scene;
        public static TreeIcon visible;
        public static TreeIcon x;

        public static void Initialize()
        {
            if (_initialized) return;
            
            _initialized = true;
            
            var type = typeof(TreeIcons);
            
            var fields = type.GetFields();
            
            foreach (var field in fields)
            {
                var icon = field.GetValue(null) as TreeIcon;

                if (icon == null)
                {
                    icon = new TreeIcon();
                    field.SetValue(null, icon);
                }

                icon.SetFieldName(field.Name);
            }
        }
    }
}