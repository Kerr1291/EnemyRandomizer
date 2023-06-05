using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{



    /////////////////////////////////////////////////////////////////////////////
    /////   


    public class ZoteBossSpawner : DefaultSpawner<ZoteBossControl>
    {
        public override bool corpseRemovedByEffect => true;

        public override string corpseRemoveEffectName => "Death Puff Med";
    }

    public class ZoteBossPrefabConfig : DefaultPrefabConfig
    {
    }



    public class ZoteBossControl : DefaultSpawnedEnemyControl
    {
        public static int nextPrecept = 1;
        public static bool preceptsEnabled;
        public int myPrecept;
        public string zoteText = "I am Zote.";
        public override string customDreamnailText => zoteText;
            


        public override void Setup(GameObject other)
        {
            base.Setup(other);

            bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
            bool cancelRoar = false;
            if (isColoBronze)
            {
                var bronze = BattleManager.StateMachine.Value as ColoBronze;
                cancelRoar = !bronze.isZoteWave;
            }

            if (cancelRoar)
            {
                var roara = control.GetState("Roar Antic");
                if (roara != null)
                {
                    roara.ChangeTransition("FINISHED", "Roar End");
                }
                else
                {
                    Dev.LogError("No roar antic on zote boss?");
                }
            }

            gameObject.DisableKillFreeze();

            if(preceptsEnabled)
            {
                MakePrecept(nextPrecept);
                nextPrecept++;
            }
        }

        protected override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if(preceptsEnabled)
            {
                return GetHP(myPrecept);
            }

            return base.GetStartingMaxHP(objectThatWillBeReplaced);
        }

        public virtual void MakePrecept(int precept)
        {
            myPrecept = precept;
            SetColor(precept);
            SetPowers(precept);
            zoteText = PreceptText.precepts[myPrecept];
        }

        public virtual void SetColor(int precept)
        {
            var baseColor = Colors.GetColor(precept);
            //var palette = Colors.GenerateTriadicColorPalette(baseColor);

            Sprite.color = baseColor;
        }

        public virtual int GetHP(int precept)
        {
            return 10 * precept;
        }

        public virtual void SetPowers(int precept)
        {
            //Powers.ConfigPrecept[precept].Invoke(this);
        }

    }

    //public static class Powers
    //{
    //    public static List<Action<ZoteBossControl>> ConfigPrecept = new List<Action<ZoteBossControl>>()
    //    {
    //        //precept 0 -- nothing
    //        (x => { 
                
    //        }),
            
    //        //precept 1 
    //        (x => {
    //            x.gameObject.
    //        }),
    //    };
    //}



    public static class Colors
    {
        public struct TriadicColorPalette
        {
            public Color baseColor;
            public Color complementaryColor;
            public Color secondaryColor1;
            public Color secondaryColor2;
        }

        public struct AnalogousColorPalette
        {
            public Color baseColor;
            public Color[] analogousColors;
        }

        public struct MonochromaticColorPalette
        {
            public Color baseColor;
            public Color[] shades;
        }

        public struct ComplementaryColorPalette
        {
            public Color baseColor;
            public Color complementaryColor;
        }

        public static Color GetRandomColor(RNG rng = null)
        {
            if(rng == null)
            {
                rng = new RNG();
                rng.Reset();
            }

            return GetColor(rng.Rand(0, 58));
        }

        public static Color GetColor(int i)
        {
            i = i % _58colors.Count;
            return _58colors[i];
        }

        public static int Count
        {
            get
            {
                return _58colors.Count;
            }
        }

        private static Color GetComplementaryColor(Color color)
        {
            float hue, saturation, value;
            Color.RGBToHSV(color, out hue, out saturation, out value);

            float complementaryHue = (hue + 0.5f) % 1.0f;
            Color complementaryColor = Color.HSVToRGB(complementaryHue, saturation, value);

            return complementaryColor;
        }

        private static Color GetSecondaryColor(Color color, float offset)
        {
            float hue, saturation, value;
            Color.RGBToHSV(color, out hue, out saturation, out value);

            float secondaryHue = (hue + offset) % 1.0f;
            Color secondaryColor = Color.HSVToRGB(secondaryHue, saturation, value);

            return secondaryColor;
        }

        private static (Color near, Color far) GetSecondaryColors(Color inputColor)
        {
            Color secondaryColor1 = GetSecondaryColor(inputColor, 1.0f / 3.0f); // One-third around the color wheel
            Color secondaryColor2 = GetSecondaryColor(inputColor, 2.0f / 3.0f); // Two-thirds around the color wheel

            return (secondaryColor1, secondaryColor2);
        }

        public static Color[] GetAnalogousColors(Color inputColor, float angle = 30f, int count = 2)
        {
            Color[] analogousColors = new Color[count];

            float hue, saturation, value;
            Color.RGBToHSV(inputColor, out hue, out saturation, out value);

            float startAngle = hue * 360f - (angle * (count - 1)) / 2f;

            for (int i = 0; i < count; i++)
            {
                float currentAngle = startAngle + angle * i;
                float normalizedAngle = Mathf.Repeat(currentAngle, 360f) / 360f;
                Color analogousColor = Color.HSVToRGB(normalizedAngle, saturation, value);
                analogousColors[i] = analogousColor;
            }

            return analogousColors;
        }

        private static Color[] GetMonochromaticShades(Color baseColor, int count)
        {
            Color[] shades = new Color[count];

            float hue, saturation, value;
            Color.RGBToHSV(baseColor, out hue, out saturation, out value);

            float valueStep = value / (count - 1);

            for (int i = 0; i < count; i++)
            {
                Color shade = Color.HSVToRGB(hue, saturation, valueStep * i);
                shades[i] = shade;
            }

            return shades;
        }

        public static TriadicColorPalette GenerateTriadicColorPalette(Color baseColor)
        {
            TriadicColorPalette colorPalette = new TriadicColorPalette();
            colorPalette.baseColor = baseColor;
            colorPalette.complementaryColor = GetComplementaryColor(baseColor);

            var secondaryColors = GetSecondaryColors(baseColor);
            colorPalette.secondaryColor1 = secondaryColors.near;
            colorPalette.secondaryColor2 = secondaryColors.far;

            return colorPalette;
        }

        public static AnalogousColorPalette GenerateAnalogousColorPalette(Color baseColor, float angle = 30f, int count = 2)
        {
            AnalogousColorPalette colorPalette = new AnalogousColorPalette();
            colorPalette.baseColor = baseColor;
            colorPalette.analogousColors = GetAnalogousColors(baseColor, angle, count);

            return colorPalette;
        }

        public static MonochromaticColorPalette GenerateMonochromaticColorPalette(Color baseColor, int count = 5)
        {
            MonochromaticColorPalette colorPalette = new MonochromaticColorPalette();
            colorPalette.baseColor = baseColor;
            colorPalette.shades = GetMonochromaticShades(baseColor, count);

            return colorPalette;
        }

        public static ComplementaryColorPalette GenerateComplementaryColorPalette(Color baseColor)
        {
            ComplementaryColorPalette colorPalette = new ComplementaryColorPalette();
            colorPalette.baseColor = baseColor;
            colorPalette.complementaryColor = GetComplementaryColor(baseColor);

            return colorPalette;
        }

        public static List<Color> _58colors = new List<Color>()
        {
            new Color(1.00f, 1.00f, 1.00f, 1f),    // white
            new Color(0.56f, 0.93f, 0.93f, 1f),    // LightCyan
            new Color(0.93f, 0.38f, 0.51f, 1f),    // HotPink
            new Color(0.80f, 0.59f, 0.00f, 1f),    // Gold
            new Color(0.98f, 0.50f, 0.45f, 1f),    // LightCoral
                
            new Color(0.93f, 0.93f, 0.93f, 1f),    // LightGray
            new Color(0.84f, 0.56f, 0.56f, 1f),    // IndianRed
            new Color(0.56f, 0.93f, 0.56f, 1f),    // LightGreen
            new Color(0.56f, 0.56f, 0.93f, 1f),    // LightBlue
            new Color(0.93f, 0.93f, 0.56f, 1f),    // LightYellow
                                                       
            new Color(0.93f, 0.56f, 0.93f, 1f),    // Plum
            new Color(0.00f, 0.00f, 0.00f, 1f),    // black
            new Color(1.00f, 0.00f, 0.00f, 1f),    // red
            new Color(0.75f, 0.75f, 0.75f, 1f),    // Silver
            new Color(0.78f, 0.08f, 0.52f, 1f),    // DeepPink
                                                       
            new Color(0.33f, 0.42f, 0.18f, 1f),    // DarkOliveGreen
            new Color(0.41f, 0.41f, 0.41f, 1f),    // DimGray
            new Color(0.91f, 0.76f, 0.65f, 1f),    // BurlyWood
            new Color(0.96f, 0.96f, 0.86f, 1f),    // Cornsilk
            new Color(0.55f, 0.47f, 0.14f, 1f),    // DarkKhaki
                                                       
            new Color(0.70f, 0.89f, 0.96f, 1f),    // LightSkyBlue
            new Color(0.89f, 0.47f, 0.20f, 1f),    // Sienna
            new Color(0.00f, 1.00f, 0.00f, 1f),    // green
            new Color(0.24f, 0.71f, 0.56f, 1f),    // MediumAquamarine
            new Color(0.58f, 0.00f, 0.83f, 1f),    // Indigo
                                                       
            new Color(0.63f, 0.08f, 0.82f, 1f),    // DarkMagenta
            new Color(0.94f, 0.50f, 0.31f, 1f),    // Salmon
            new Color(0.29f, 0.59f, 0.82f, 1f),    // SteelBlue
            new Color(0.85f, 0.53f, 0.10f, 1f),    // DarkGoldenrod
            new Color(0.40f, 0.80f, 0.67f, 1f),    // MediumSeaGreen
                                                       
            new Color(0.68f, 0.85f, 0.90f, 1f),    // LightSteelBlue
            new Color(0.27f, 0.51f, 0.71f, 1f),    // DodgerBlue
            new Color(0.00f, 0.00f, 1.00f, 1f),    // blue
            new Color(0.95f, 0.50f, 0.00f, 1f),    // Orange
            new Color(0.85f, 0.45f, 0.82f, 1f),    // Orchid
                                                       
            new Color(0.88f, 0.65f, 0.47f, 1f),    // Tan
            new Color(0.44f, 0.53f, 0.56f, 1f),    // CadetBlue
            new Color(0.99f, 0.94f, 0.81f, 1f),    // Cornsilk
            new Color(0.65f, 0.00f, 0.18f, 1f),    // Maroon
            new Color(0.49f, 0.99f, 0.83f, 1f),    // MintCream
                                                       
            new Color(0.86f, 0.44f, 0.58f, 1f),    // RosyBrown
            new Color(0.84f, 0.28f, 0.28f, 1f),    // Firebrick
            new Color(0.24f, 0.67f, 0.24f, 1f),    // LimeGreen
            new Color(0.24f, 0.53f, 0.96f, 1f),    // MediumBlue
            new Color(0.96f, 0.96f, 0.96f, 1f),    // WhiteSmoke

            new Color(0.69f, 0.13f, 0.13f, 1f),    // DarkRed
            new Color(0.65f, 0.16f, 0.49f, 1f),    // MediumPurple
            new Color(0.73f, 0.56f, 0.75f, 1f),    // MediumOrchid
            new Color(0.00f, 0.75f, 0.75f, 1f),    // Aqua
            new Color(0.87f, 0.63f, 0.87f, 1f),    // Thistle

            new Color(0.75f, 0.50f, 0.31f, 1f),    // Chocolate
            new Color(0.51f, 0.81f, 0.93f, 1f),    // SkyBlue
            new Color(0.43f, 0.50f, 0.82f, 1f),    // DarkSlateBlue
            new Color(0.90f, 0.63f, 0.56f, 1f),    // Coral
            new Color(0.11f, 0.67f, 0.55f, 1f),    // SeaGreen
                
            new Color(0.82f, 0.37f, 0.57f, 1f),    // MediumVioletRed
            new Color(0.77f, 0.60f, 0.43f, 1f),    // Peru
            new Color(0.56f, 0.56f, 0.56f, 1f),    // DarkGray
        };

        public static void ShiftSpriteHue(this tk2dSprite sprite, TriadicColorPalette colorPalette)
        {
            Texture2D texture = (Texture2D)sprite.Collection.spriteDefinitions[sprite.spriteId].materialInst.mainTexture;
            Color[] pixels = texture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color originalColor = pixels[i];
                Color newColor = ShiftHue(originalColor, colorPalette);
                pixels[i] = newColor;
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }

        public static Color ShiftHue(Color originalColor, TriadicColorPalette colorPalette)
        {
            float h, s, v;
            Color.RGBToHSV(originalColor, out h, out s, out v);

            Color closestColor = GetClosestColor(originalColor, colorPalette);

            if (closestColor != Color.clear)
            {
                float closestHue;
                Color.RGBToHSV(closestColor, out closestHue, out s, out v);
                h = closestHue; // Use the hue of the closest color for shifting
            }
            else
            {
                h = Mathf.Repeat(h + 0.1f, 1f); // Hue shift
            }

            return Color.HSVToRGB(h, s, v);
        }

        public static Color GetClosestColor(Color targetColor, TriadicColorPalette colorPalette)
        {
            Color closestColor = Color.clear;
            float closestDistance = Mathf.Infinity;

            List<Color> paletteColors = new List<Color>()
            {
                colorPalette.baseColor,
                colorPalette.complementaryColor,
                colorPalette.secondaryColor1,
                colorPalette.secondaryColor2
            };

            foreach (Color paletteColor in paletteColors)
            {
                float distance = ColorDistance(targetColor, paletteColor);

                if (distance < closestDistance)
                {
                    closestColor = paletteColor;
                    closestDistance = distance;
                }
            }

            return closestColor;
        }

        public static float ColorDistance(Color color1, Color color2)
        {
            return Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b);
        }

        //public static void ShiftSpriteHueForAll(this tk2dSpriteAnimator animator, TriadicColorPalette colorPalette)
        //{
        //    tk2dSpriteAnimationClip[] clips = animator.Library.clips;

        //    foreach (tk2dSpriteAnimationClip clip in clips)
        //    {
        //        foreach (tk2dSpriteAnimationFrame frame in clip.frames)
        //        {
        //            Texture2D texture = (Texture2D)frame.spriteCollection.spriteDefinitions[frame.spriteId].materialInst.mainTexture;
        //            Color[] pixels = texture.GetPixels();

        //            for (int i = 0; i < pixels.Length; i++)
        //            {
        //                Color originalColor = pixels[i];
        //                Color newColor = ShiftHue(originalColor, colorPalette);
        //                pixels[i] = newColor;
        //            }

        //            texture.SetPixels(pixels);
        //            texture.Apply();
        //        }
        //    }
        //}


        //public static void ShiftSpriteHue(this tk2dSpriteAnimator animator, TriadicColorPalette colorPalette)
        //{
        //    tk2dSpriteAnimationClip[] clips = animator.Library.clips;

        //    foreach (tk2dSpriteAnimationClip clip in clips)
        //    {
        //        foreach (tk2dSpriteAnimationFrame frame in clip.frames)
        //        {
        //            string cacheKey = frame.spriteCollection.name + frame.spriteId + colorPalette.baseColor.ColorToHex();

        //            tk2dSpriteCollectionData originalCollection = frame.spriteCollection;
        //            if (!cachedFrames.TryGetValue(cacheKey, out var newCollection))
        //            {
        //                // Create a new separate sprite collection for the animator
        //                newCollection = ScriptableObject.Instantiate(originalCollection);
        //            }

        //            frame.spriteCollection = newCollection;

        //            var originalDef = frame.spriteCollection.spriteDefinitions[frame.spriteId];
        //            if (!cachedDefs.TryGetValue(cacheKey, out var newDef))
        //            {
        //                // Create a new separate sprite collection for the animator
        //                newDef = ScriptableObject.Instantiate(originalCollection);
        //            }

        //            Material copiedMaterialInst = GetRecolorMaterial(colorPalette, frame);

        //            // Assign the copied material instance to the frame
        //            frame.spriteCollection.spriteDefinitions[frame.spriteId].materialInst = copiedMaterialInst;
        //        }
        //    }
        //}

        //public static Dictionary<string, tk2dSpriteCollectionData> cachedFrames = new Dictionary<string, tk2dSpriteCollectionData>();
        //public static Dictionary<string, tk2dSpriteDefinition> cachedDefs = new Dictionary<string, tk2dSpriteDefinition>();
        //public static Dictionary<string, Material> cachedMaterials = new Dictionary<string, Material>();
        //public static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
        //private static Material GetRecolorMaterial(TriadicColorPalette colorPalette, tk2dSpriteAnimationFrame frame)
        //{
        //    string cacheKey = frame.spriteCollection.name + frame.spriteId + colorPalette.baseColor.ColorToHex();

        //    Material materialInst = frame.spriteCollection.spriteDefinitions[frame.spriteId].materialInst;
        //    Texture2D texture = (Texture2D)materialInst.mainTexture;

        //    if (!cachedMaterials.TryGetValue(cacheKey, out var copiedMaterialInst))
        //    {
        //        // Create a copy of the material instance
        //        copiedMaterialInst = new Material(materialInst);
        //    }

        //    if (!cachedTextures.TryGetValue(cacheKey, out var copiedTextureInst))
        //    {
        //        // Create a copy of the texture instance
        //        copiedTextureInst = new Texture2D(texture.width, texture.height);

        //        Color[] pixels = texture.GetPixels();

        //        for (int i = 0; i < pixels.Length; i++)
        //        {
        //            Color originalColor = pixels[i];
        //            Color newColor = ShiftHue(originalColor, colorPalette);
        //            pixels[i] = newColor;
        //        }

        //        copiedTextureInst.name = texture.name + "_RecoloredTo_" + colorPalette.baseColor.ColorToHex();

        //        // Apply the modified pixels to the copied material instance
        //        copiedTextureInst.SetPixels(pixels);
        //        copiedTextureInst.Apply();
        //    }

        //    copiedMaterialInst.mainTexture = copiedTextureInst;
        //    return copiedMaterialInst;
        //}

        //public static Color ShiftHue(Color originalColor, TriadicColorPalette colorPalette)
        //{
        //    float h, s, v;
        //    Color.RGBToHSV(originalColor, out h, out s, out v);

        //    Color closestColor = GetClosestColor(originalColor, colorPalette);

        //    if (closestColor != Color.clear)
        //    {
        //        float closestHue;
        //        Color.RGBToHSV(closestColor, out closestHue, out s, out v);
        //        h = closestHue; // Use the hue of the closest color for shifting
        //    }
        //    else
        //    {
        //        h = Mathf.Repeat(h + 0.1f, 1f); // Hue shift
        //    }

        //    return Color.HSVToRGB(h, s, v);
        //}

    }


    public static class PreceptText
    {
        //TODO:
        public static List<string> precepts = new List<string>()
        {
            //Precept One: 'Always Win Your Battles'. Losing a battle earns you nothing and teaches you nothing.Win your battles, or don't engage in them at all!

            //Precept Two: 'Never Let Them Laugh at You'. Fools laugh at everything, even at their superiors. But beware, laughter isn't harmless! Laughter spreads like a disease, and soon everyone is laughing at you. You need to strike at the source of this perverse merriment quickly to stop it from spreading.

            //Precept Three: 'Always Be Rested'. Fighting and adventuring take their toll on your body.When you rest, your body strengthens and repairs itself. The longer you rest, the stronger you become.

            //Precept Four: 'Forget Your Past'. The past is painful, and thinking about your past can only bring you misery. Think about something else instead, such as the future, or some food.

            //Precept Five: 'Strength Beats Strength'. Is your opponent strong? No matter! Simply overcome their strength with even more strength, and they'll soon be defeated.

            //Precept Six: 'Choose Your Own Fate'. Our elders teach that our fate is chosen for us before we are even born. I disagree.

            //Precept Seven: 'Mourn Not the Dead'. When we die, do things get better for us or worse? There's no way to tell, so we shouldn't bother mourning.Or celebrating for that matter.

            //Precept Eight: 'Travel Alone'. You can rely on nobody, and nobody will always be loyal. Therefore, nobody should be your constant companion.

            //Precept Nine: 'Keep Your Home Tidy'. Your home is where you keep your most prized possession - yourself.Therefore, you should make an effort to keep it nice and clean.

            //Precept Ten: 'Keep Your Weapon Sharp'. I make sure that my weapon, 'Life Ender', is kept well-sharpened at all times. This makes it much easier to cut things.

            //Precept Eleven: 'Mothers Will Always Betray You'. This Precept explains itself.

            //Precept Twelve: 'Keep Your Cloak Dry'. If your cloak gets wet, dry it as soon as you can. Wearing wet cloaks is unpleasant, and can lead to illness.

            //Precept Thirteen: 'Never Be Afraid'. Fear can only hold you back. Facing your fears can be a tremendous effort. Therefore, you should just not be afraid in the first place.

            //Precept Fourteen: 'Respect Your Superiors'. If someone is your superior in strength or intellect or both, you need to show them your respect.Don't ignore them or laugh at them.

            //Precept Fifteen: 'One Foe, One Blow'. You should only use a single blow to defeat an enemy.Any more is a waste. Also, by counting your blows as you fight, you'll know how many foes you've defeated.

            //Precept Sixteen: 'Don't Hesitate'. Once you've made a decision, carry it out and don't look back. You'll achieve much more this way.

            //Precept Seventeen: 'Believe In Your Strength'. Others may doubt you, but there's someone you can always trust. Yourself. Make sure to believe in your own strength, and you will never falter.

            //Precept Eighteen: 'Seek Truth in the Darkness'. This Precept also explains itself.

            //Precept Nineteen: 'If You Try, Succeed'. If you're going to attempt something, make sure you achieve it. If you do not succeed, then you have actually failed! Avoid this at all costs.

            //Precept Twenty: 'Speak Only the Truth'. When speaking to someone, it is courteous and also efficient to speak truthfully.Beware though that speaking truthfully may make you enemies.This is something you'll have to bear.

            //Precept Twenty-One: 'Be Aware of Your Surroundings'. Don't just walk along staring at the ground! You need to look up every so often, to make sure nothing takes you by surprise.

            //Precept Twenty-Two: 'Abandon the Nest'. As soon as I could, I left my birthplace and made my way out into the world.Do not linger in the nest. There is nothing for you there.

            //Precept Twenty-Three: 'Identify the Foe's Weak Point'. Every foe you encounter has a weak point, such as a crack in their shell or being asleep. You must constantly be alert and scrutinising your enemy to detect their weakness!

            //Precept Twenty-Four: 'Strike the Foe's Weak Point'. Once you have identified your foe's weak point as per the previous Precept, strike it. This will instantly destroy them.

            //Precept Twenty-Five: 'Protect Your Own Weak Point'. Be aware that your foe will try to identify your weak point, so you must protect it.The best protection? Never having a weak point in the first place.

            //Precept Twenty-Six: 'Don't Trust Your Reflection'. When peering at certain shining surfaces, you may see a copy of your own face. The face will mimic your movements and seems similar to your own, but I don't think it can be trusted.

            //Precept Twenty-Seven: 'Eat As Much As You Can'. When having a meal, eat as much as you possibly can.This gives you extra energy, and means you can eat less frequently.

            //Precept Twenty-Eight: 'Don't Peer Into the Darkness'. If you peer into the darkness and can't see anything for too long, your mind will start to linger over old memories.Memories are to be avoided, as per Precept Four.

            //Precept Twenty-Nine: 'Develop Your Sense of Direction'. It's easy to get lost when travelling through winding, twisting caverns. Having a good sense of direction is like having a magical map inside of your head. Very useful.

            //Precept Thirty: 'Never Accept a Promise'. Spurn the promises of others, as they are always broken. Promises of love or betrothal are to be avoided especially.

            //Precept Thirty-One: 'Disease Lives Inside of Dirt'. You'll get sick if you spend too much time in filthy places. If you are staying in someone else's home, demand the highest level of cleanliness from your host.

            //Precept Thirty-Two: 'Names Have Power'. Names have power, and so to name something is to grant it power. I myself named my nail 'Life Ender'. Do not steal the name I came up with! Invent your own!

            //Precept Thirty-Three: 'Show the Enemy No Respect'. Being gallant to your enemies is no virtue! If someone opposes you, they don't deserve respect or kindness or mercy.

            //Precept Thirty-Four: 'Don't Eat Immediately Before Sleeping'. This can cause restlessness and indigestion. It's just common sense.

            //Precept Thirty-Five: 'Up is Up, Down is Down'. If you fall over in the darkness, it can be easy to lose your bearing and forget which way is up.Keep this Precept in mind!

            //Precept Thirty-Six: 'Eggshells are brittle'. Once again, this Precept explains itself.

            //Precept Thirty-Seven: 'Borrow, But Do Not Lend'. If you lend and are repayed, you gain nothing.If you borrow but do not repay, you gain everything.

            //Precept Thirty-Eight: 'Beware the Mysterious Force'. A mysterious force bears down on us from above, pushing us downwards.If you spend too long in the air, the force will crush you against the ground and destroy you.Beware!

            //Precept Thirty-Nine: 'Eat Quickly and Drink Slowly'. Your body is a delicate thing and you must fuel it with great deliberation.Food must go in as fast as possible, but fluids at a slower rate.

            //Precept Forty: 'Obey No Law But Your Own'. Laws written by others may inconvenience you or be a burden.Let your own desires be the only law.

            //Precept Forty-One: 'Learn to Detect Lies'. When others speak, they usually lie.Scrutinise and question them relentlessly until they reveal their deceit.

            //Precept Forty-Two: 'Spend Geo When You Have It'. Some will cling onto their Geo, even taking it into the dirt with them when they die.It is better to spend it when you can, so you can enjoy various things in life.

            //Precept Forty-Three: 'Never Forgive'. If someone asks forgiveness of you, for instance a brother of yours, always deny it.That brother, or whoever it is, doesn't deserve such a thing.

            //Precept Forty-Four: 'You Can Not Breathe Water'. Water is refreshing, but if you try to breathe it you are in for a nasty shock.

            //Precept Forty-Five: 'One Thing Is Not Another'. This one should be obvious, but I've had others try to argue that one thing, which is clearly what it is and not something else, is actually some other thing, which it isn't.Stay on your guard!

            //Precept Forty-Six: 'The World is Smaller Than You Think'. When young, you tend to think that the world is vast, huge, gigantic.It's only natural. Unfortunately, it's actually quite a lot smaller than that. I can say this, now having travelled everywhere in the land.

            //Precept Forty-Seven: 'Make Your Own Weapon'. Only you know exactly what is needed in your weapon. I myself fashioned 'Life Ender' from shellwood at a young age. It has never failed me.Nor I it.

            //Precept Forty-Eight: 'Be Careful With Fire'. Fire is a type of hot spirit that dances about recklessly.It can warm you and provide light, but it will also singe your shell if it gets too close.

            //Precept Forty-Nine: 'Statues are Meaningless'. Do not honour them! No one has ever made a statue of you or I, so why should we pay them any attention?

            //Precept Fifty: 'Don't Linger on Mysteries'. Some things in this world appear to us as puzzles. Or enigmas. If the meaning behind something is not immediately evident though, don't waste any time thinking about it.Just move on.

            //Precept Fifty-One: 'Nothing is Harmless'. Given the chance, everything in this world will hurt you. Friends, foes, monsters, uneven paths. Be suspicious of them all.

            //Precept Fifty-Two: 'Beware the Jealousy of Fathers'. Fathers believe that because they created us we must serve them and never exceed their capabilities. If you wish to forge your own path, you must vanquish your father.Or simply abandon him.

            //Precept Fifty-Three: 'Do Not Steal the Desires of Others'. Every creature keeps their desires locked up inside of themselves. If you catch a glimpse of another's desires, resist the urge to claim them as your own. It will not lead you to happiness.

            //Precept Fifty-Four: 'If You Lock Something Away, Keep the Key'. Nothing should be locked away for ever, so hold onto your keys.You will eventually return and unlock everything you hid away.

            //Precept Fifty-Five: 'Bow to No-one'. There are those in this world who would impose their will on others. They claim ownership over your food, your land, your body, and even your thoughts! They have done nothing to earn these things. Never bow to them, and make sure to disobey their commands.

            //Precept Fifty-Six: 'Do Not Dream'. Dreams are dangerous things. Strange ideas, not your own, can worm their way into your mind.But if you resist those ideas, sickness will wrack your body! Best not to dream at all, like me.

            //Precept Fifty-Seven: 'Obey All Precepts'. Most importantly, you must commit all of these Precepts to memory and obey them all unfailingly. Including this one! Hmm.Have you truly listened to everything I've said? Let's start again and repeat the 'Fifty-Seven Precepts of Zote'

        };
    }




    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



}



















//public class GGZoteCorpseFixer : MonoBehaviour
//{
//    IEnumerator Start()
//    {
//        yield return new WaitUntil(() => gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid") != null);
//        Dev.Log("trying to fix corpse white flash");
//        var whiteScreenEffect = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid").gameObject;

//        if (whiteScreenEffect == null)
//        {
//            Dev.LogError("Failed to find white screen effect");
//            yield break;
//        }

//        var corpseFSM = gameObject.LocateMyFSM("Control");
//        var fsm = whiteScreenEffect.LocateMyFSM("FSM");

//        while (fsm == null)

//        {
//            fsm = whiteScreenEffect.LocateMyFSM("FSM");
//            yield return null;
//        }

//        while (fsm.GetState("Init") == null)
//            yield return null;

//        while (fsm.GetState("Down") == null)
//            yield return null;


//        if (fsm.ActiveStateName == "Init")
//            fsm.SendEvent("UP");

//        while (fsm.ActiveStateName != "Upped")
//            yield return null;

//        HeroController.instance.RegainControl();
//        HeroController.instance.StartAnimationControl();

//        if (fsm.ActiveStateName == "Upped")
//            fsm.SendEvent("DOWN");


//        while (corpseFSM.ActiveStateName != "Notify")
//            yield return null;


//        while (corpseFSM.ActiveStateName == "Notify")
//        {
//            corpseFSM.SendEvent("CORPSE END");
//            yield return null;
//        }

//        Destroy(this);
//        yield break;
//    }
//}