// 此文件由 GameResAssetProcessor 自动生成
// 请勿手动修改此文件

namespace HotUpdate.Utility
{
public static class AddressableKeys
{
    public const string Animations_File = "Assets/GameRes/Animations";
    public const string Art_File = "Assets/GameRes/Art";
    public const string Audio_File = "Assets/GameRes/Audio";
    public const string Data_File = "Assets/GameRes/Data";
    public const string Prefabs_File = "Assets/GameRes/Prefabs";
    public const string Scenes_File = "Assets/GameRes/Scenes";
    public const string ScriptableObjects_File = "Assets/GameRes/ScriptableObjects";
    public static class Animations
    {
        public const string Character_File = "Assets/GameRes/Animations/Character";
        public const string MiniGame_File = "Assets/GameRes/Animations/MiniGame";
        public const string UI_File = "Assets/GameRes/Animations/UI";
        public const string World_File = "Assets/GameRes/Animations/World";
        public static class Character
        {
            public const string PlayerController_Controller = "Assets/GameRes/Animations/Character/PlayerController.controller";
            public enum CharacterName
            {
                PlayerController
            }
            public static string GetCharacter(string path) => "Assets/GameRes/Animations/Character/" + path + ".controller";
        }
        public static class MiniGame
        {
            public const string IceBreaker_File = "Assets/GameRes/Animations/MiniGame/IceBreaker";
            public static class IceBreaker
            {
                public const string OpenAnim_File = "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim";
                public const string Player_File = "Assets/GameRes/Animations/MiniGame/IceBreaker/Player";
                public static class OpenAnim
                {
                    public const string Controller_File = "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/Controller";
                    public const string OpenAnim_Anim = "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/OpenAnim.anim";
                    public const string OpenAnimEnd_Anim = "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/OpenAnimEnd.anim";
                    public static class Controller
                    {
                        public const string OpenAnim_Controller = "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/Controller/OpenAnim.controller";
                        public enum ControllerName
                        {
                            OpenAnim
                        }
                        public static string GetController(string path) => "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/Controller/" + path + ".controller";
                    }
                    public enum OpenAnimName
                    {
                        Controller,
                        OpenAnim,
                        OpenAnimEnd
                    }
                    public static string GetOpenAnim(string path) => "Assets/GameRes/Animations/MiniGame/IceBreaker/OpenAnim/" + path + "";
                }
                public static class Player
                {
                    public const string Attack_Anim = "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/Attack.anim";
                    public const string Controller_File = "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/Controller";
                    public const string Idle_Anim = "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/Idle.anim";
                    public static class Controller
                    {
                        public const string Player_Controller = "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/Controller/Player.controller";
                        public enum ControllerName
                        {
                            Player
                        }
                        public static string GetController(string path) => "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/Controller/" + path + ".controller";
                    }
                    public enum PlayerName
                    {
                        Attack,
                        Controller,
                        Idle
                    }
                    public static string GetPlayer(string path) => "Assets/GameRes/Animations/MiniGame/IceBreaker/Player/" + path + ".anim";
                }
                public enum IceBreakerName
                {
                    OpenAnim,
                    Player
                }
                public static string GetIceBreaker(string path) => "Assets/GameRes/Animations/MiniGame/IceBreaker/" + path + "";
            }
            public enum MiniGameName
            {
                IceBreaker
            }
            public static string GetMiniGame(string path) => "Assets/GameRes/Animations/MiniGame/" + path + "";
        }
        public static class UI
        {
            public const string AdWindow_File = "Assets/GameRes/Animations/UI/AdWindow";
            public const string GlitchEffect_File = "Assets/GameRes/Animations/UI/GlitchEffect";
            public const string GlitchWindow_File = "Assets/GameRes/Animations/UI/GlitchWindow";
            public const string IceBreakerAd_File = "Assets/GameRes/Animations/UI/IceBreakerAd";
            public const string Loading_File = "Assets/GameRes/Animations/UI/Loading";
            public const string Login_File = "Assets/GameRes/Animations/UI/Login";
            public const string Narrator_File = "Assets/GameRes/Animations/UI/Narrator";
            public const string PauseUIButton_File = "Assets/GameRes/Animations/UI/PauseUIButton";
            public const string SkipButton_File = "Assets/GameRes/Animations/UI/SkipButton";
            public static class AdWindow
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/AdWindow/Controller";
                public const string Enter_Anim = "Assets/GameRes/Animations/UI/AdWindow/Enter.anim";
                public static class Controller
                {
                    public const string AdWindow_Controller = "Assets/GameRes/Animations/UI/AdWindow/Controller/AdWindow.controller";
                    public enum ControllerName
                    {
                        AdWindow
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/AdWindow/Controller/" + path + ".controller";
                }
                public enum AdWindowName
                {
                    Controller,
                    Enter
                }
                public static string GetAdWindow(string path) => "Assets/GameRes/Animations/UI/AdWindow/" + path + "";
            }
            public static class GlitchEffect
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/GlitchEffect/Controller";
                public const string GlitchEffect_Anim = "Assets/GameRes/Animations/UI/GlitchEffect/GlitchEffect.anim";
                public static class Controller
                {
                    public const string GlitchEffect_Controller = "Assets/GameRes/Animations/UI/GlitchEffect/Controller/GlitchEffect.controller";
                    public enum ControllerName
                    {
                        GlitchEffect
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/GlitchEffect/Controller/" + path + ".controller";
                }
                public enum GlitchEffectName
                {
                    Controller,
                    GlitchEffect
                }
                public static string GetGlitchEffect(string path) => "Assets/GameRes/Animations/UI/GlitchEffect/" + path + "";
            }
            public static class GlitchWindow
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/GlitchWindow/Controller";
                public const string GlitchWindow_Anim = "Assets/GameRes/Animations/UI/GlitchWindow/GlitchWindow.anim";
                public static class Controller
                {
                    public const string GlitchWindow_Controller = "Assets/GameRes/Animations/UI/GlitchWindow/Controller/GlitchWindow.controller";
                    public enum ControllerName
                    {
                        GlitchWindow
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/GlitchWindow/Controller/" + path + ".controller";
                }
                public enum GlitchWindowName
                {
                    Controller,
                    GlitchWindow
                }
                public static string GetGlitchWindow(string path) => "Assets/GameRes/Animations/UI/GlitchWindow/" + path + "";
            }
            public static class IceBreakerAd
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/IceBreakerAd/Controller";
                public const string Enter_Anim = "Assets/GameRes/Animations/UI/IceBreakerAd/Enter.anim";
                public const string IceBreakerAd_en_US_Anim = "Assets/GameRes/Animations/UI/IceBreakerAd/IceBreakerAd_en-US.anim";
                public const string IceBreakerAd_zh_CN_Anim = "Assets/GameRes/Animations/UI/IceBreakerAd/IceBreakerAd_zh-CN.anim";
                public static class Controller
                {
                    public const string IceBreakerAd_en_US_Controller = "Assets/GameRes/Animations/UI/IceBreakerAd/Controller/IceBreakerAd_en-US.controller";
                    public const string IceBreakerAd_zh_CN_Controller = "Assets/GameRes/Animations/UI/IceBreakerAd/Controller/IceBreakerAd_zh-CN.controller";
                    public enum ControllerName
                    {
                        IceBreakerAd_en_US,
                        IceBreakerAd_zh_CN
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/IceBreakerAd/Controller/" + path + ".controller";
                }
                public enum IceBreakerAdName
                {
                    Controller,
                    Enter,
                    IceBreakerAd_en_US,
                    IceBreakerAd_zh_CN
                }
                public static string GetIceBreakerAd(string path) => "Assets/GameRes/Animations/UI/IceBreakerAd/" + path + "";
            }
            public static class Loading
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/Loading/Controller";
                public const string loading_Anim = "Assets/GameRes/Animations/UI/Loading/loading.anim";
                public static class Controller
                {
                    public const string Loading_Controller = "Assets/GameRes/Animations/UI/Loading/Controller/Loading.controller";
                    public enum ControllerName
                    {
                        Loading
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/Loading/Controller/" + path + ".controller";
                }
                public enum LoadingName
                {
                    Controller,
                    loading
                }
                public static string GetLoading(string path) => "Assets/GameRes/Animations/UI/Loading/" + path + "";
            }
            public static class Login
            {
                public const string controller_File = "Assets/GameRes/Animations/UI/Login/controller";
                public const string First_Anim = "Assets/GameRes/Animations/UI/Login/First.anim";
                public const string First2Second_Anim = "Assets/GameRes/Animations/UI/Login/First2Second.anim";
                public const string Second_Anim = "Assets/GameRes/Animations/UI/Login/Second.anim";
                public const string SelectServer_Anim = "Assets/GameRes/Animations/UI/Login/SelectServer.anim";
                public const string start_File = "Assets/GameRes/Animations/UI/Login/start";
                public static class controller
                {
                    public const string Login_Controller = "Assets/GameRes/Animations/UI/Login/controller/Login.controller";
                    public enum controllerName
                    {
                        Login
                    }
                    public static string Getcontroller(string path) => "Assets/GameRes/Animations/UI/Login/controller/" + path + ".controller";
                }
                public enum LoginName
                {
                    controller,
                    First,
                    First2Second,
                    Second,
                    SelectServer,
                    start
                }
                public static string GetLogin(string path) => "Assets/GameRes/Animations/UI/Login/" + path + "";
            }
            public static class Narrator
            {
                public const string Controller_File = "Assets/GameRes/Animations/UI/Narrator/Controller";
                public const string Narrator1Enter_Anim = "Assets/GameRes/Animations/UI/Narrator/Narrator1Enter.anim";
                public const string Narrator1Idle_Anim = "Assets/GameRes/Animations/UI/Narrator/Narrator1Idle.anim";
                public static class Controller
                {
                    public const string Icon_Controller = "Assets/GameRes/Animations/UI/Narrator/Controller/Icon.controller";
                    public enum ControllerName
                    {
                        Icon
                    }
                    public static string GetController(string path) => "Assets/GameRes/Animations/UI/Narrator/Controller/" + path + ".controller";
                }
                public enum NarratorName
                {
                    Controller,
                    Narrator1Enter,
                    Narrator1Idle
                }
                public static string GetNarrator(string path) => "Assets/GameRes/Animations/UI/Narrator/" + path + "";
            }
            public static class PauseUIButton
            {
                public const string OptionUIButton_Controller = "Assets/GameRes/Animations/UI/PauseUIButton/OptionUIButton.controller";
                public const string PauseUIButton_Controller = "Assets/GameRes/Animations/UI/PauseUIButton/PauseUIButton.controller";
                public enum PauseUIButtonName
                {
                    OptionUIButton,
                    PauseUIButton
                }
                public static string GetPauseUIButton(string path) => "Assets/GameRes/Animations/UI/PauseUIButton/" + path + ".controller";
            }
            public static class SkipButton
            {
                public const string SkipButton_Controller = "Assets/GameRes/Animations/UI/SkipButton/SkipButton.controller";
                public enum SkipButtonName
                {
                    SkipButton
                }
                public static string GetSkipButton(string path) => "Assets/GameRes/Animations/UI/SkipButton/" + path + ".controller";
            }
            public enum UIName
            {
                AdWindow,
                GlitchEffect,
                GlitchWindow,
                IceBreakerAd,
                Loading,
                Login,
                Narrator,
                PauseUIButton,
                SkipButton
            }
            public static string GetUI(string path) => "Assets/GameRes/Animations/UI/" + path + "";
        }
        public static class World
        {
            public const string ScriptRain_File = "Assets/GameRes/Animations/World/ScriptRain";
            public static class ScriptRain
            {
                public const string Normal_Anim = "Assets/GameRes/Animations/World/ScriptRain/Normal.anim";
                public const string ScriptRain_Controller = "Assets/GameRes/Animations/World/ScriptRain/ScriptRain.controller";
                public enum ScriptRainName
                {
                    Normal,
                    ScriptRain
                }
                public static string GetScriptRain(string path) => "Assets/GameRes/Animations/World/ScriptRain/" + path + ".anim";
            }
            public enum WorldName
            {
                ScriptRain
            }
            public static string GetWorld(string path) => "Assets/GameRes/Animations/World/" + path + "";
        }
        public enum AnimationsName
        {
            Character,
            MiniGame,
            UI,
            World
        }
        public static string GetAnimations(string path) => "Assets/GameRes/Animations/" + path + "";
    }
    public static class Art
    {
        public const string Common_File = "Assets/GameRes/Art/Common";
        public const string Font_File = "Assets/GameRes/Art/Font";
        public const string Materials_File = "Assets/GameRes/Art/Materials";
        public const string Playthrough3_File = "Assets/GameRes/Art/Playthrough3";
        public const string RenderTexture_File = "Assets/GameRes/Art/RenderTexture";
        public static class Common
        {
            public const string TipsGradientFade_Png = "Assets/GameRes/Art/Common/TipsGradientFade.png";
            public enum CommonName
            {
                TipsGradientFade
            }
            public static string GetCommon(string path) => "Assets/GameRes/Art/Common/" + path + ".png";
        }
        public static class Font
        {
            public const string Cartoon_File = "Assets/GameRes/Art/Font/Cartoon";
            public const string ipix_12px_SDF_Asset = "Assets/GameRes/Art/Font/ipix_12px SDF.asset";
            public const string ipix_12px_Ttf = "Assets/GameRes/Art/Font/ipix_12px.ttf";
            public const string SmileySans_Oblique_SDF_Asset = "Assets/GameRes/Art/Font/SmileySans-Oblique SDF.asset";
            public const string SmileySans_Oblique_Ttf = "Assets/GameRes/Art/Font/SmileySans-Oblique.ttf";
            public const string SourceHanSans_Medium_SDF_OutLine_Asset = "Assets/GameRes/Art/Font/SourceHanSans-Medium SDF OutLine.asset";
            public const string SourceHanSans_Medium_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSans-Medium SDF.asset";
            public const string SourceHanSans_Medium_Otf = "Assets/GameRes/Art/Font/SourceHanSans-Medium.otf";
            public const string SourceHanSerifSC_Bold_SDF_Underlay_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Bold SDF Underlay.asset";
            public const string SourceHanSerifSC_Bold_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Bold SDF.asset";
            public const string SourceHanSerifSC_Bold_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-Bold.otf";
            public const string SourceHanSerifSC_ExtraLight_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-ExtraLight SDF.asset";
            public const string SourceHanSerifSC_ExtraLight_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-ExtraLight.otf";
            public const string SourceHanSerifSC_Heavy_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Heavy SDF.asset";
            public const string SourceHanSerifSC_Heavy_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-Heavy.otf";
            public const string SourceHanSerifSC_Light_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Light SDF.asset";
            public const string SourceHanSerifSC_Light_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-Light.otf";
            public const string SourceHanSerifSC_Medium_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Medium SDF.asset";
            public const string SourceHanSerifSC_Medium_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-Medium.otf";
            public const string SourceHanSerifSC_Regular_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-Regular SDF.asset";
            public const string SourceHanSerifSC_Regular_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-Regular.otf";
            public const string SourceHanSerifSC_SemiBold_SDF_Asset = "Assets/GameRes/Art/Font/SourceHanSerifSC-SemiBold SDF.asset";
            public const string SourceHanSerifSC_SemiBold_Otf = "Assets/GameRes/Art/Font/SourceHanSerifSC-SemiBold.otf";
            public static class Cartoon
            {
                public const string MuseoModerno_CriticalNum_Red_64_Dark_SDF_Atlas_Png = "Assets/GameRes/Art/Font/Cartoon/MuseoModerno-CriticalNum_Red_64_Dark SDF Atlas.png";
                public const string MuseoModerno_CriticalNum_Red_64_Light_SDF_Atlas_Png = "Assets/GameRes/Art/Font/Cartoon/MuseoModerno-CriticalNum_Red_64_Light SDF Atlas.png";
                public const string MuseoModerno_CriticalNum_Transpar_46_SDF_Atlas_Png = "Assets/GameRes/Art/Font/Cartoon/MuseoModerno-CriticalNum_Transpar_46 SDF Atlas.png";
                public const string MuseoModerno_ExtraBold_SDF_Asset = "Assets/GameRes/Art/Font/Cartoon/MuseoModerno-ExtraBold SDF.asset";
                public const string MuseoModerno_ExtraBold_Ttf = "Assets/GameRes/Art/Font/Cartoon/MuseoModerno-ExtraBold.ttf";
                public const string Quicksand_Bold_SDF_Asset = "Assets/GameRes/Art/Font/Cartoon/Quicksand-Bold SDF.asset";
                public const string Quicksand_Bold_Ttf = "Assets/GameRes/Art/Font/Cartoon/Quicksand-Bold.ttf";
                public const string Quicksand_SemiBold_SDF_Asset = "Assets/GameRes/Art/Font/Cartoon/Quicksand-SemiBold SDF.asset";
                public const string Quicksand_SemiBold_Ttf = "Assets/GameRes/Art/Font/Cartoon/Quicksand-SemiBold.ttf";
                public const string Rubik_Medium_SDF_Asset = "Assets/GameRes/Art/Font/Cartoon/Rubik-Medium SDF.asset";
                public const string Rubik_Medium_Ttf = "Assets/GameRes/Art/Font/Cartoon/Rubik-Medium.ttf";
                public const string Rubik_SemiBold_SDF_Asset = "Assets/GameRes/Art/Font/Cartoon/Rubik-SemiBold SDF.asset";
                public const string Rubik_SemiBold_Ttf = "Assets/GameRes/Art/Font/Cartoon/Rubik-SemiBold.ttf";
                public enum CartoonName
                {
                    MuseoModerno_CriticalNum_Red_64_Dark_SDF_Atlas,
                    MuseoModerno_CriticalNum_Red_64_Light_SDF_Atlas,
                    MuseoModerno_CriticalNum_Transpar_46_SDF_Atlas,
                    MuseoModerno_ExtraBold_SDF,
                    MuseoModerno_ExtraBold,
                    Quicksand_Bold_SDF,
                    Quicksand_Bold,
                    Quicksand_SemiBold_SDF,
                    Quicksand_SemiBold,
                    Rubik_Medium_SDF,
                    Rubik_Medium,
                    Rubik_SemiBold_SDF,
                    Rubik_SemiBold
                }
                public static string GetCartoon(string path) => "Assets/GameRes/Art/Font/Cartoon/" + path + ".png";
            }
            public enum FontName
            {
                Cartoon,
                ipix_12px_SDF,
                ipix_12px,
                SmileySans_Oblique_SDF,
                SmileySans_Oblique,
                SourceHanSans_Medium_SDF_OutLine,
                SourceHanSans_Medium_SDF,
                SourceHanSans_Medium,
                SourceHanSerifSC_Bold_SDF_Underlay,
                SourceHanSerifSC_Bold_SDF,
                SourceHanSerifSC_Bold,
                SourceHanSerifSC_ExtraLight_SDF,
                SourceHanSerifSC_ExtraLight,
                SourceHanSerifSC_Heavy_SDF,
                SourceHanSerifSC_Heavy,
                SourceHanSerifSC_Light_SDF,
                SourceHanSerifSC_Light,
                SourceHanSerifSC_Medium_SDF,
                SourceHanSerifSC_Medium,
                SourceHanSerifSC_Regular_SDF,
                SourceHanSerifSC_Regular,
                SourceHanSerifSC_SemiBold_SDF,
                SourceHanSerifSC_SemiBold
            }
            public static string GetFont(string path) => "Assets/GameRes/Art/Font/" + path + "";
        }
        public static class Materials
        {
            public const string Baguette_Mat = "Assets/GameRes/Art/Materials/Baguette.mat";
            public const string Green_Mat = "Assets/GameRes/Art/Materials/Green.mat";
            public const string IceBreakerSkybox_Mat = "Assets/GameRes/Art/Materials/IceBreakerSkybox.mat";
            public const string TrailPlayerAttack_Mat = "Assets/GameRes/Art/Materials/TrailPlayerAttack.mat";
            public const string TrailPlayerIdle_Mat = "Assets/GameRes/Art/Materials/TrailPlayerIdle.mat";
            public const string Water_Mat = "Assets/GameRes/Art/Materials/Water.mat";
            public enum MaterialsName
            {
                Baguette,
                Green,
                IceBreakerSkybox,
                TrailPlayerAttack,
                TrailPlayerIdle,
                Water
            }
            public static string GetMaterials(string path) => "Assets/GameRes/Art/Materials/" + path + ".mat";
        }
        public static class Playthrough3
        {
            public const string Common_File = "Assets/GameRes/Art/Playthrough3/Common";
            public const string Loading_File = "Assets/GameRes/Art/Playthrough3/Loading";
            public const string Login_File = "Assets/GameRes/Art/Playthrough3/Login";
            public const string Menu_File = "Assets/GameRes/Art/Playthrough3/Menu";
            public static class Common
            {
                public const string _00_Components_1_Png = "Assets/GameRes/Art/Playthrough3/Common/00_Components_1.png";
                public const string quit_Png = "Assets/GameRes/Art/Playthrough3/Common/quit.png";
                public const string user_Png = "Assets/GameRes/Art/Playthrough3/Common/user.png";
                public enum CommonName
                {
                    _00_Components_1,
                    quit,
                    user
                }
                public static string GetCommon(string path) => "Assets/GameRes/Art/Playthrough3/Common/" + path + ".png";
            }
            public static class Loading
            {
                public const string LoadingBg_Png = "Assets/GameRes/Art/Playthrough3/Loading/LoadingBg.png";
                public enum LoadingName
                {
                    LoadingBg
                }
                public static string GetLoading(string path) => "Assets/GameRes/Art/Playthrough3/Loading/" + path + ".png";
            }
            public static class Login
            {
                public const string LoginBg_Png = "Assets/GameRes/Art/Playthrough3/Login/LoginBg.png";
                public const string LoginBg2_Png = "Assets/GameRes/Art/Playthrough3/Login/LoginBg2.png";
                public const string LoginBg2ServerBtn_Png = "Assets/GameRes/Art/Playthrough3/Login/LoginBg2ServerBtn.png";
                public const string Title_Png = "Assets/GameRes/Art/Playthrough3/Login/Title.png";
                public enum LoginName
                {
                    LoginBg,
                    LoginBg2,
                    LoginBg2ServerBtn,
                    Title
                }
                public static string GetLogin(string path) => "Assets/GameRes/Art/Playthrough3/Login/" + path + ".png";
            }
            public static class Menu
            {
                public const string MenuBg_Png = "Assets/GameRes/Art/Playthrough3/Menu/MenuBg.png";
                public enum MenuName
                {
                    MenuBg
                }
                public static string GetMenu(string path) => "Assets/GameRes/Art/Playthrough3/Menu/" + path + ".png";
            }
            public enum Playthrough3Name
            {
                Common,
                Loading,
                Login,
                Menu
            }
            public static string GetPlaythrough3(string path) => "Assets/GameRes/Art/Playthrough3/" + path + "";
        }
        public static class RenderTexture
        {
            public const string HackPreviewRenderTexture_RenderTexture = "Assets/GameRes/Art/RenderTexture/HackPreviewRenderTexture.renderTexture";
            public const string Video_Render_Texture_RenderTexture = "Assets/GameRes/Art/RenderTexture/Video Render Texture.renderTexture";
            public const string Water_Render_Texture_0_RenderTexture = "Assets/GameRes/Art/RenderTexture/Water Render Texture 0.renderTexture";
            public const string Water_Render_Texture_1_RenderTexture = "Assets/GameRes/Art/RenderTexture/Water Render Texture 1.renderTexture";
            public enum RenderTextureName
            {
                HackPreviewRenderTexture,
                Video_Render_Texture,
                Water_Render_Texture_0,
                Water_Render_Texture_1
            }
            public static string GetRenderTexture(string path) => "Assets/GameRes/Art/RenderTexture/" + path + ".renderTexture";
        }
        public enum ArtName
        {
            Common,
            Font,
            Materials,
            Playthrough3,
            RenderTexture
        }
        public static string GetArt(string path) => "Assets/GameRes/Art/" + path + "";
    }
    public static class Audio
    {
        public const string BGM_File = "Assets/GameRes/Audio/BGM";
        public const string Casual_Game_Sounds_U6_File = "Assets/GameRes/Audio/Casual Game Sounds U6";
        public const string Sound_File = "Assets/GameRes/Audio/Sound";
        public const string Voice_File = "Assets/GameRes/Audio/Voice";
        public static class BGM
        {
            public const string Circulation_Mp3 = "Assets/GameRes/Audio/BGM/Circulation.mp3";
            public const string City_of_Night_Mp3 = "Assets/GameRes/Audio/BGM/City of Night.mp3";
            public const string Fruit_Machine_Mp3 = "Assets/GameRes/Audio/BGM/Fruit Machine.mp3";
            public const string Hate__Birth_and_Humanity_Mp3 = "Assets/GameRes/Audio/BGM/Hate, Birth and Humanity.mp3";
            public const string Lantern_Mp3 = "Assets/GameRes/Audio/BGM/Lantern.mp3";
            public const string ReplaceInTheFuture_File = "Assets/GameRes/Audio/BGM/ReplaceInTheFuture";
            public const string Rest_time_Mp3 = "Assets/GameRes/Audio/BGM/Rest time.mp3";
            public static class ReplaceInTheFuture
            {
                public const string TheSongOfGuanyu_Mp3 = "Assets/GameRes/Audio/BGM/ReplaceInTheFuture/TheSongOfGuanyu.mp3";
                public enum ReplaceInTheFutureName
                {
                    TheSongOfGuanyu
                }
                public static string GetReplaceInTheFuture(string path) => "Assets/GameRes/Audio/BGM/ReplaceInTheFuture/" + path + ".mp3";
            }
            public enum BGMName
            {
                Circulation,
                City_of_Night,
                Fruit_Machine,
                Hate__Birth_and_Humanity,
                Lantern,
                ReplaceInTheFuture,
                Rest_time
            }
            public static string GetBGM(string path) => "Assets/GameRes/Audio/BGM/" + path + ".mp3";
        }
        public static class Casual_Game_Sounds_U6
        {
            public const string CasualGameSounds_File = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds";
            public static class CasualGameSounds
            {
                public const string DM_CGS_01_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-01.wav";
                public const string DM_CGS_02_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-02.wav";
                public const string DM_CGS_03_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-03.wav";
                public const string DM_CGS_04_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-04.wav";
                public const string DM_CGS_05_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-05.wav";
                public const string DM_CGS_06_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-06.wav";
                public const string DM_CGS_07_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-07.wav";
                public const string DM_CGS_08_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-08.wav";
                public const string DM_CGS_09_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-09.wav";
                public const string DM_CGS_10_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-10.wav";
                public const string DM_CGS_11_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-11.wav";
                public const string DM_CGS_12_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-12.wav";
                public const string DM_CGS_13_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-13.wav";
                public const string DM_CGS_14_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-14.wav";
                public const string DM_CGS_15_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-15.wav";
                public const string DM_CGS_16_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-16.wav";
                public const string DM_CGS_17_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-17.wav";
                public const string DM_CGS_18_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-18.wav";
                public const string DM_CGS_19_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-19.wav";
                public const string DM_CGS_20_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-20.wav";
                public const string DM_CGS_21_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-21.wav";
                public const string DM_CGS_22_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-22.wav";
                public const string DM_CGS_23_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-23.wav";
                public const string DM_CGS_24_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-24.wav";
                public const string DM_CGS_25_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-25.wav";
                public const string DM_CGS_26_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-26.wav";
                public const string DM_CGS_27_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-27.wav";
                public const string DM_CGS_28_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-28.wav";
                public const string DM_CGS_29_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-29.wav";
                public const string DM_CGS_30_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-30.wav";
                public const string DM_CGS_31_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-31.wav";
                public const string DM_CGS_32_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-32.wav";
                public const string DM_CGS_33_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-33.wav";
                public const string DM_CGS_34_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-34.wav";
                public const string DM_CGS_35_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-35.wav";
                public const string DM_CGS_36_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-36.wav";
                public const string DM_CGS_37_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-37.wav";
                public const string DM_CGS_38_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-38.wav";
                public const string DM_CGS_39_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-39.wav";
                public const string DM_CGS_40_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-40.wav";
                public const string DM_CGS_41_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-41.wav";
                public const string DM_CGS_42_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-42.wav";
                public const string DM_CGS_43_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-43.wav";
                public const string DM_CGS_44_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-44.wav";
                public const string DM_CGS_45_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-45.wav";
                public const string DM_CGS_46_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-46.wav";
                public const string DM_CGS_47_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-47.wav";
                public const string DM_CGS_48_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-48.wav";
                public const string DM_CGS_49_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-49.wav";
                public const string DM_CGS_50_Wav = "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/DM-CGS-50.wav";
                public enum CasualGameSoundsName
                {
                    DM_CGS_01,
                    DM_CGS_02,
                    DM_CGS_03,
                    DM_CGS_04,
                    DM_CGS_05,
                    DM_CGS_06,
                    DM_CGS_07,
                    DM_CGS_08,
                    DM_CGS_09,
                    DM_CGS_10,
                    DM_CGS_11,
                    DM_CGS_12,
                    DM_CGS_13,
                    DM_CGS_14,
                    DM_CGS_15,
                    DM_CGS_16,
                    DM_CGS_17,
                    DM_CGS_18,
                    DM_CGS_19,
                    DM_CGS_20,
                    DM_CGS_21,
                    DM_CGS_22,
                    DM_CGS_23,
                    DM_CGS_24,
                    DM_CGS_25,
                    DM_CGS_26,
                    DM_CGS_27,
                    DM_CGS_28,
                    DM_CGS_29,
                    DM_CGS_30,
                    DM_CGS_31,
                    DM_CGS_32,
                    DM_CGS_33,
                    DM_CGS_34,
                    DM_CGS_35,
                    DM_CGS_36,
                    DM_CGS_37,
                    DM_CGS_38,
                    DM_CGS_39,
                    DM_CGS_40,
                    DM_CGS_41,
                    DM_CGS_42,
                    DM_CGS_43,
                    DM_CGS_44,
                    DM_CGS_45,
                    DM_CGS_46,
                    DM_CGS_47,
                    DM_CGS_48,
                    DM_CGS_49,
                    DM_CGS_50
                }
                public static string GetCasualGameSounds(string path) => "Assets/GameRes/Audio/Casual Game Sounds U6/CasualGameSounds/" + path + ".wav";
            }
            public enum Casual_Game_Sounds_U6Name
            {
                CasualGameSounds
            }
            public static string GetCasual_Game_Sounds_U6(string path) => "Assets/GameRes/Audio/Casual Game Sounds U6/" + path + "";
        }
        public static class Sound
        {
            public const string MouseClick_Wav = "Assets/GameRes/Audio/Sound/MouseClick.wav";
            public enum SoundName
            {
                MouseClick
            }
            public static string GetSound(string path) => "Assets/GameRes/Audio/Sound/" + path + ".wav";
        }
        public static class Voice
        {
            public const string CatTalk1_Mp3 = "Assets/GameRes/Audio/Voice/CatTalk1.mp3";
            public const string CatTalk2_Mp3 = "Assets/GameRes/Audio/Voice/CatTalk2.mp3";
            public enum VoiceName
            {
                CatTalk1,
                CatTalk2
            }
            public static string GetVoice(string path) => "Assets/GameRes/Audio/Voice/" + path + ".mp3";
        }
        public enum AudioName
        {
            BGM,
            Casual_Game_Sounds_U6,
            Sound,
            Voice
        }
        public static string GetAudio(string path) => "Assets/GameRes/Audio/" + path + "";
    }
    public static class Data
    {
        public const string Dialogue_File = "Assets/GameRes/Data/Dialogue";
        public const string Enemy_File = "Assets/GameRes/Data/Enemy";
        public const string MainMenu_File = "Assets/GameRes/Data/MainMenu";
        public const string MiniGame_File = "Assets/GameRes/Data/MiniGame";
        public static class Dialogue
        {
            public const string ExampleDialogue_Json = "Assets/GameRes/Data/Dialogue/ExampleDialogue.json";
            public const string SkipPlot1_Json = "Assets/GameRes/Data/Dialogue/SkipPlot1.json";
            public const string test_dialogue_Json = "Assets/GameRes/Data/Dialogue/test_dialogue.json";
            public enum DialogueName
            {
                ExampleDialogue,
                SkipPlot1,
                test_dialogue
            }
            public static string GetDialogue(string path) => "Assets/GameRes/Data/Dialogue/" + path + ".json";
        }
        public static class Enemy
        {
            public const string EnemyData_Json = "Assets/GameRes/Data/Enemy/EnemyData.json";
            public enum EnemyName
            {
                EnemyData
            }
            public static string GetEnemy(string path) => "Assets/GameRes/Data/Enemy/" + path + ".json";
        }
        public static class MainMenu
        {
            public const string MainMenuData_Json = "Assets/GameRes/Data/MainMenu/MainMenuData.json";
            public enum MainMenuName
            {
                MainMenuData
            }
            public static string GetMainMenu(string path) => "Assets/GameRes/Data/MainMenu/" + path + ".json";
        }
        public static class MiniGame
        {
            public const string IceBreaker_File = "Assets/GameRes/Data/MiniGame/IceBreaker";
            public static class IceBreaker
            {
                public const string IceBreakerData_Json = "Assets/GameRes/Data/MiniGame/IceBreaker/IceBreakerData.json";
                public enum IceBreakerName
                {
                    IceBreakerData
                }
                public static string GetIceBreaker(string path) => "Assets/GameRes/Data/MiniGame/IceBreaker/" + path + ".json";
            }
            public enum MiniGameName
            {
                IceBreaker
            }
            public static string GetMiniGame(string path) => "Assets/GameRes/Data/MiniGame/" + path + "";
        }
        public enum DataName
        {
            Dialogue,
            Enemy,
            MainMenu,
            MiniGame
        }
        public static string GetData(string path) => "Assets/GameRes/Data/" + path + "";
    }
    public static class Prefabs
    {
        public const string Camera_File = "Assets/GameRes/Prefabs/Camera";
        public const string Character_File = "Assets/GameRes/Prefabs/Character";
        public const string Dialogue_File = "Assets/GameRes/Prefabs/Dialogue";
        public const string DialogueBubbles_File = "Assets/GameRes/Prefabs/DialogueBubbles";
        public const string Effect_File = "Assets/GameRes/Prefabs/Effect";
        public const string Empty_Prefab = "Assets/GameRes/Prefabs/Empty.prefab";
        public const string Enemy_File = "Assets/GameRes/Prefabs/Enemy";
        public const string Environment_File = "Assets/GameRes/Prefabs/Environment";
        public const string IceBreaker_File = "Assets/GameRes/Prefabs/IceBreaker";
        public const string TriggerDialogueBubble_File = "Assets/GameRes/Prefabs/TriggerDialogueBubble";
        public const string UI_File = "Assets/GameRes/Prefabs/UI";
        public const string Video_File = "Assets/GameRes/Prefabs/Video";
        public const string World_File = "Assets/GameRes/Prefabs/World";
        public static class Camera
        {
            public const string InitCamera_Prefab = "Assets/GameRes/Prefabs/Camera/InitCamera.prefab";
            public const string ToiletVillageCamera_Prefab = "Assets/GameRes/Prefabs/Camera/ToiletVillageCamera.prefab";
            public enum CameraName
            {
                InitCamera,
                ToiletVillageCamera
            }
            public static string GetCamera(string path) => "Assets/GameRes/Prefabs/Camera/" + path + ".prefab";
        }
        public static class Character
        {
            public const string NPC_File = "Assets/GameRes/Prefabs/Character/NPC";
            public const string Player_File = "Assets/GameRes/Prefabs/Character/Player";
            public static class NPC
            {
                public const string DaoshiLaoBa_Prefab = "Assets/GameRes/Prefabs/Character/NPC/DaoshiLaoBa.prefab";
                public const string NPC1_Prefab = "Assets/GameRes/Prefabs/Character/NPC/NPC1.prefab";
                public enum NPCName
                {
                    DaoshiLaoBa,
                    NPC1
                }
                public static string GetNPC(string path) => "Assets/GameRes/Prefabs/Character/NPC/" + path + ".prefab";
            }
            public static class Player
            {
                public const string Player1_Prefab = "Assets/GameRes/Prefabs/Character/Player/Player1.prefab";
                public enum PlayerName
                {
                    Player1
                }
                public static string GetPlayer(string path) => "Assets/GameRes/Prefabs/Character/Player/" + path + ".prefab";
            }
            public enum CharacterName
            {
                NPC,
                Player
            }
            public static string GetCharacter(string path) => "Assets/GameRes/Prefabs/Character/" + path + "";
        }
        public static class Dialogue
        {
            public const string Dialogue0_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue0.prefab";
            public const string Dialogue1_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue1.prefab";
            public const string Dialogue2_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue2.prefab";
            public const string Dialogue3_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue3.prefab";
            public const string Dialogue4_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue4.prefab";
            public const string Dialogue5_Prefab = "Assets/GameRes/Prefabs/Dialogue/Dialogue5.prefab";
            public enum DialogueName
            {
                Dialogue0,
                Dialogue1,
                Dialogue2,
                Dialogue3,
                Dialogue4,
                Dialogue5
            }
            public static string GetDialogue(string path) => "Assets/GameRes/Prefabs/Dialogue/" + path + ".prefab";
        }
        public static class DialogueBubbles
        {
            public const string DialogueBubble0_Prefab = "Assets/GameRes/Prefabs/DialogueBubbles/DialogueBubble0.prefab";
            public const string DialogueBubble1_Prefab = "Assets/GameRes/Prefabs/DialogueBubbles/DialogueBubble1.prefab";
            public const string DialogueBubble2_Prefab = "Assets/GameRes/Prefabs/DialogueBubbles/DialogueBubble2.prefab";
            public const string DialogueBubble3_Prefab = "Assets/GameRes/Prefabs/DialogueBubbles/DialogueBubble3.prefab";
            public enum DialogueBubblesName
            {
                DialogueBubble0,
                DialogueBubble1,
                DialogueBubble2,
                DialogueBubble3
            }
            public static string GetDialogueBubbles(string path) => "Assets/GameRes/Prefabs/DialogueBubbles/" + path + ".prefab";
        }
        public static class Effect
        {
            public const string ScriptRain_Prefab = "Assets/GameRes/Prefabs/Effect/ScriptRain.prefab";
            public enum EffectName
            {
                ScriptRain
            }
            public static string GetEffect(string path) => "Assets/GameRes/Prefabs/Effect/" + path + ".prefab";
        }
        public static class Enemy
        {
            public const string BugToiletVillage_Prefab = "Assets/GameRes/Prefabs/Enemy/BugToiletVillage.prefab";
            public const string Manager_File = "Assets/GameRes/Prefabs/Enemy/Manager";
            public static class Manager
            {
                public const string EnemyManager_Prefab = "Assets/GameRes/Prefabs/Enemy/Manager/EnemyManager.prefab";
                public enum ManagerName
                {
                    EnemyManager
                }
                public static string GetManager(string path) => "Assets/GameRes/Prefabs/Enemy/Manager/" + path + ".prefab";
            }
            public enum EnemyName
            {
                BugToiletVillage,
                Manager
            }
            public static string GetEnemy(string path) => "Assets/GameRes/Prefabs/Enemy/" + path + ".prefab";
        }
        public static class Environment
        {
            public const string _2D_File = "Assets/GameRes/Prefabs/Environment/2D";
            public const string _3D_File = "Assets/GameRes/Prefabs/Environment/3D";
            public static class _2D
            {
                public const string PF_Village_Props___Anvil_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Anvil 01.prefab";
                public const string PF_Village_Props___Apple_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Apple 01.prefab";
                public const string PF_Village_Props___Archery_Target_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Archery Target 01.prefab";
                public const string PF_Village_Props___Arrow_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Arrow.prefab";
                public const string PF_Village_Props___Banner_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Banner.prefab";
                public const string PF_Village_Props___Barrel_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Barrel.prefab";
                public const string PF_Village_Props___Barricade_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Barricade.prefab";
                public const string PF_Village_Props___Basket_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Basket.prefab";
                public const string PF_Village_Props___Billboard_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Billboard.prefab";
                public const string PF_Village_Props___Board_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Board.prefab";
                public const string PF_Village_Props___Bounding_Platform_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bounding Platform 01.prefab";
                public const string PF_Village_Props___Bread_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bread 01.prefab";
                public const string PF_Village_Props___Brick_Wall_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Brick Wall 01.prefab";
                public const string PF_Village_Props___Bucket_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bucket 01.prefab";
                public const string PF_Village_Props___Bush_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bush 01.prefab";
                public const string PF_Village_Props___Bush_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bush 02.prefab";
                public const string PF_Village_Props___Bush_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Bush 03.prefab";
                public const string PF_Village_Props___Campfire_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Campfire 01.prefab";
                public const string PF_Village_Props___Cauldron_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Cauldron.prefab";
                public const string PF_Village_Props___Chair_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Chair.prefab";
                public const string PF_Village_Props___Chest_Golden_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Chest Golden.prefab";
                public const string PF_Village_Props___Chest_Iron_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Chest Iron.prefab";
                public const string PF_Village_Props___Chest_Silver_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Chest Silver.prefab";
                public const string PF_Village_Props___Chest_Wooden_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Chest Wooden.prefab";
                public const string PF_Village_Props___Clother_Hanger_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Clother Hanger.prefab";
                public const string PF_Village_Props___Crate_Large_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Crate Large.prefab";
                public const string PF_Village_Props___Crate_Small_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Crate Small.prefab";
                public const string PF_Village_Props___Cup_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Cup.prefab";
                public const string PF_Village_Props___Elevator_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Elevator.prefab";
                public const string PF_Village_Props___Fence_A_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Fence A.prefab";
                public const string PF_Village_Props___Fence_B_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Fence B.prefab";
                public const string PF_Village_Props___Fire_Bowl_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Fire Bowl.prefab";
                public const string PF_Village_Props___Flower_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Flower 01.prefab";
                public const string PF_Village_Props___Flower_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Flower 02.prefab";
                public const string PF_Village_Props___Grain_Box_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grain Box.prefab";
                public const string PF_Village_Props___Grass_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 01.prefab";
                public const string PF_Village_Props___Grass_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 02.prefab";
                public const string PF_Village_Props___Grass_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 03.prefab";
                public const string PF_Village_Props___Grass_04_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 04.prefab";
                public const string PF_Village_Props___Grass_05_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 05.prefab";
                public const string PF_Village_Props___Grass_06_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 06.prefab";
                public const string PF_Village_Props___Grass_07_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 07.prefab";
                public const string PF_Village_Props___Grass_08_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 08.prefab";
                public const string PF_Village_Props___Grass_09_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Grass 09.prefab";
                public const string PF_Village_Props___Gravestone_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gravestone 01.prefab";
                public const string PF_Village_Props___Gravestone_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gravestone 02.prefab";
                public const string PF_Village_Props___Gravestone_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gravestone 03.prefab";
                public const string PF_Village_Props___Gravestone_04_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gravestone 04.prefab";
                public const string PF_Village_Props___Gunny_Bag_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gunny Bag 01.prefab";
                public const string PF_Village_Props___Gunny_Bag_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Gunny Bag 02.prefab";
                public const string PF_Village_Props___Hammer_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Hammer.prefab";
                public const string PF_Village_Props___Hay_Bale_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Hay Bale.prefab";
                public const string PF_Village_Props___Hay_Fork_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Hay Fork.prefab";
                public const string PF_Village_Props___Hay_Pile_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Hay Pile.prefab";
                public const string PF_Village_Props___Heavy_Sword_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Heavy Sword.prefab";
                public const string PF_Village_Props___Jump_Platform_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Jump Platform 01.prefab";
                public const string PF_Village_Props___Ladder_01_Side_L_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Ladder 01 Side L.prefab";
                public const string PF_Village_Props___Ladder_01_Side_R_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Ladder 01 Side R.prefab";
                public const string PF_Village_Props___Ladder_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Ladder 01.prefab";
                public const string PF_Village_Props___Ladder_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Ladder 02.prefab";
                public const string PF_Village_Props___Log_Bench_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Log Bench.prefab";
                public const string PF_Village_Props___Obstacle_Platform_Large_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform Large 01.prefab";
                public const string PF_Village_Props___Obstacle_Platform_Large_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform Large 02.prefab";
                public const string PF_Village_Props___Obstacle_Platform_X16_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform X16.prefab";
                public const string PF_Village_Props___Obstacle_Platform_X24_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform X24.prefab";
                public const string PF_Village_Props___Obstacle_Platform_X32_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform X32.prefab";
                public const string PF_Village_Props___Obstacle_Platform_X40_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Obstacle Platform X40.prefab";
                public const string PF_Village_Props___Platform_01_L_X1_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 01 L X1.prefab";
                public const string PF_Village_Props___Platform_01_L_X2_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 01 L X2.prefab";
                public const string PF_Village_Props___Platform_01_R_X1_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 01 R X1.prefab";
                public const string PF_Village_Props___Platform_01_R_X2_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 01 R X2.prefab";
                public const string PF_Village_Props___Platform_02_X1_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 02 X1.prefab";
                public const string PF_Village_Props___Platform_02_X2_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 02 X2.prefab";
                public const string PF_Village_Props___Platform_02_X3_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 02 X3.prefab";
                public const string PF_Village_Props___Platform_02_X4_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Platform 02 X4.prefab";
                public const string PF_Village_Props___Pot_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Pot 01.prefab";
                public const string PF_Village_Props___Pot_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Pot 02.prefab";
                public const string PF_Village_Props___Pot_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Pot 03.prefab";
                public const string PF_Village_Props___Pumpkin_A_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Pumpkin A.prefab";
                public const string PF_Village_Props___Pumpkin_B_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Pumpkin B.prefab";
                public const string PF_Village_Props___Road_Lamp_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Road Lamp.prefab";
                public const string PF_Village_Props___Road_Sign_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Road Sign 01.prefab";
                public const string PF_Village_Props___Road_Sign_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Road Sign 02.prefab";
                public const string PF_Village_Props___Rock_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Rock 01.prefab";
                public const string PF_Village_Props___Rock_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Rock 02.prefab";
                public const string PF_Village_Props___Rock_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Rock 03.prefab";
                public const string PF_Village_Props___Scarecrow_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Scarecrow 01.prefab";
                public const string PF_Village_Props___Seesaw_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Seesaw 01.prefab";
                public const string PF_Village_Props___Sign_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Sign 01.prefab";
                public const string PF_Village_Props___Slope_Platform_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Slope Platform 01.prefab";
                public const string PF_Village_Props___Spear_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Spear.prefab";
                public const string PF_Village_Props___Spike_Ball_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Spike Ball.prefab";
                public const string PF_Village_Props___Spike_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Spike.prefab";
                public const string PF_Village_Props___Stairs_X24_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stairs X24.prefab";
                public const string PF_Village_Props___Stairs_X32_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stairs X32.prefab";
                public const string PF_Village_Props___Stairs_X40_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stairs X40.prefab";
                public const string PF_Village_Props___Stairs_X48_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stairs X48.prefab";
                public const string PF_Village_Props___Stairs_X64_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stairs X64.prefab";
                public const string PF_Village_Props___Stall_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stall.prefab";
                public const string PF_Village_Props___Statue_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Statue.prefab";
                public const string PF_Village_Props___Stone_of_Recall_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stone of Recall.prefab";
                public const string PF_Village_Props___Stump_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Stump.prefab";
                public const string PF_Village_Props___Sunflower_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Sunflower 01.prefab";
                public const string PF_Village_Props___Sunflower_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Sunflower 02.prefab";
                public const string PF_Village_Props___Sunflower_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Sunflower 03.prefab";
                public const string PF_Village_Props___Sword_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Sword.prefab";
                public const string PF_Village_Props___Table_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Table.prefab";
                public const string PF_Village_Props___Torch_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Torch.prefab";
                public const string PF_Village_Props___Training_Dummy_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Training Dummy.prefab";
                public const string PF_Village_Props___Tree_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Tree 01.prefab";
                public const string PF_Village_Props___Tree_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Tree 02.prefab";
                public const string PF_Village_Props___Weapon_Rack_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Weapon Rack.prefab";
                public const string PF_Village_Props___Well_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Well.prefab";
                public const string PF_Village_Props___Wheat_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheat 01.prefab";
                public const string PF_Village_Props___Wheat_02_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheat 02.prefab";
                public const string PF_Village_Props___Wheat_03_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheat 03.prefab";
                public const string PF_Village_Props___Wheat_04_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheat 04.prefab";
                public const string PF_Village_Props___Wheat_05_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheat 05.prefab";
                public const string PF_Village_Props___Wheelbarrow_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wheelbarrow.prefab";
                public const string PF_Village_Props___White_Bottle_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - White Bottle.prefab";
                public const string PF_Village_Props___Wine_Bottle_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wine Bottle.prefab";
                public const string PF_Village_Props___Wood_Log_01_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wood Log 01.prefab";
                public const string PF_Village_Props___Wood_Logs_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wood Logs.prefab";
                public const string PF_Village_Props___Wooden_Bridge_X6_Prefab = "Assets/GameRes/Prefabs/Environment/2D/PF Village Props - Wooden Bridge X6.prefab";
                public const string TX_Village_Props___Kettle_Prefab = "Assets/GameRes/Prefabs/Environment/2D/TX Village Props - Kettle.prefab";
                public enum _2DName
                {
                    PF_Village_Props___Anvil_01,
                    PF_Village_Props___Apple_01,
                    PF_Village_Props___Archery_Target_01,
                    PF_Village_Props___Arrow,
                    PF_Village_Props___Banner,
                    PF_Village_Props___Barrel,
                    PF_Village_Props___Barricade,
                    PF_Village_Props___Basket,
                    PF_Village_Props___Billboard,
                    PF_Village_Props___Board,
                    PF_Village_Props___Bounding_Platform_01,
                    PF_Village_Props___Bread_01,
                    PF_Village_Props___Brick_Wall_01,
                    PF_Village_Props___Bucket_01,
                    PF_Village_Props___Bush_01,
                    PF_Village_Props___Bush_02,
                    PF_Village_Props___Bush_03,
                    PF_Village_Props___Campfire_01,
                    PF_Village_Props___Cauldron,
                    PF_Village_Props___Chair,
                    PF_Village_Props___Chest_Golden,
                    PF_Village_Props___Chest_Iron,
                    PF_Village_Props___Chest_Silver,
                    PF_Village_Props___Chest_Wooden,
                    PF_Village_Props___Clother_Hanger,
                    PF_Village_Props___Crate_Large,
                    PF_Village_Props___Crate_Small,
                    PF_Village_Props___Cup,
                    PF_Village_Props___Elevator,
                    PF_Village_Props___Fence_A,
                    PF_Village_Props___Fence_B,
                    PF_Village_Props___Fire_Bowl,
                    PF_Village_Props___Flower_01,
                    PF_Village_Props___Flower_02,
                    PF_Village_Props___Grain_Box,
                    PF_Village_Props___Grass_01,
                    PF_Village_Props___Grass_02,
                    PF_Village_Props___Grass_03,
                    PF_Village_Props___Grass_04,
                    PF_Village_Props___Grass_05,
                    PF_Village_Props___Grass_06,
                    PF_Village_Props___Grass_07,
                    PF_Village_Props___Grass_08,
                    PF_Village_Props___Grass_09,
                    PF_Village_Props___Gravestone_01,
                    PF_Village_Props___Gravestone_02,
                    PF_Village_Props___Gravestone_03,
                    PF_Village_Props___Gravestone_04,
                    PF_Village_Props___Gunny_Bag_01,
                    PF_Village_Props___Gunny_Bag_02,
                    PF_Village_Props___Hammer,
                    PF_Village_Props___Hay_Bale,
                    PF_Village_Props___Hay_Fork,
                    PF_Village_Props___Hay_Pile,
                    PF_Village_Props___Heavy_Sword,
                    PF_Village_Props___Jump_Platform_01,
                    PF_Village_Props___Ladder_01_Side_L,
                    PF_Village_Props___Ladder_01_Side_R,
                    PF_Village_Props___Ladder_01,
                    PF_Village_Props___Ladder_02,
                    PF_Village_Props___Log_Bench,
                    PF_Village_Props___Obstacle_Platform_Large_01,
                    PF_Village_Props___Obstacle_Platform_Large_02,
                    PF_Village_Props___Obstacle_Platform_X16,
                    PF_Village_Props___Obstacle_Platform_X24,
                    PF_Village_Props___Obstacle_Platform_X32,
                    PF_Village_Props___Obstacle_Platform_X40,
                    PF_Village_Props___Platform_01_L_X1,
                    PF_Village_Props___Platform_01_L_X2,
                    PF_Village_Props___Platform_01_R_X1,
                    PF_Village_Props___Platform_01_R_X2,
                    PF_Village_Props___Platform_02_X1,
                    PF_Village_Props___Platform_02_X2,
                    PF_Village_Props___Platform_02_X3,
                    PF_Village_Props___Platform_02_X4,
                    PF_Village_Props___Pot_01,
                    PF_Village_Props___Pot_02,
                    PF_Village_Props___Pot_03,
                    PF_Village_Props___Pumpkin_A,
                    PF_Village_Props___Pumpkin_B,
                    PF_Village_Props___Road_Lamp,
                    PF_Village_Props___Road_Sign_01,
                    PF_Village_Props___Road_Sign_02,
                    PF_Village_Props___Rock_01,
                    PF_Village_Props___Rock_02,
                    PF_Village_Props___Rock_03,
                    PF_Village_Props___Scarecrow_01,
                    PF_Village_Props___Seesaw_01,
                    PF_Village_Props___Sign_01,
                    PF_Village_Props___Slope_Platform_01,
                    PF_Village_Props___Spear,
                    PF_Village_Props___Spike_Ball,
                    PF_Village_Props___Spike,
                    PF_Village_Props___Stairs_X24,
                    PF_Village_Props___Stairs_X32,
                    PF_Village_Props___Stairs_X40,
                    PF_Village_Props___Stairs_X48,
                    PF_Village_Props___Stairs_X64,
                    PF_Village_Props___Stall,
                    PF_Village_Props___Statue,
                    PF_Village_Props___Stone_of_Recall,
                    PF_Village_Props___Stump,
                    PF_Village_Props___Sunflower_01,
                    PF_Village_Props___Sunflower_02,
                    PF_Village_Props___Sunflower_03,
                    PF_Village_Props___Sword,
                    PF_Village_Props___Table,
                    PF_Village_Props___Torch,
                    PF_Village_Props___Training_Dummy,
                    PF_Village_Props___Tree_01,
                    PF_Village_Props___Tree_02,
                    PF_Village_Props___Weapon_Rack,
                    PF_Village_Props___Well,
                    PF_Village_Props___Wheat_01,
                    PF_Village_Props___Wheat_02,
                    PF_Village_Props___Wheat_03,
                    PF_Village_Props___Wheat_04,
                    PF_Village_Props___Wheat_05,
                    PF_Village_Props___Wheelbarrow,
                    PF_Village_Props___White_Bottle,
                    PF_Village_Props___Wine_Bottle,
                    PF_Village_Props___Wood_Log_01,
                    PF_Village_Props___Wood_Logs,
                    PF_Village_Props___Wooden_Bridge_X6,
                    TX_Village_Props___Kettle
                }
                public static string Get_2D(string path) => "Assets/GameRes/Prefabs/Environment/2D/" + path + ".prefab";
            }
            public static class _3D
            {
                public const string Toilet_Prefab = "Assets/GameRes/Prefabs/Environment/3D/Toilet.prefab";
                public enum _3DName
                {
                    Toilet
                }
                public static string Get_3D(string path) => "Assets/GameRes/Prefabs/Environment/3D/" + path + ".prefab";
            }
            public enum EnvironmentName
            {
                _2D,
                _3D
            }
            public static string GetEnvironment(string path) => "Assets/GameRes/Prefabs/Environment/" + path + "";
        }
        public static class IceBreaker
        {
            public const string IceBreakerCamera_Prefab = "Assets/GameRes/Prefabs/IceBreaker/IceBreakerCamera.prefab";
            public const string IceBreakerManager_Prefab = "Assets/GameRes/Prefabs/IceBreaker/IceBreakerManager.prefab";
            public const string IceBreakerPlayer_Prefab = "Assets/GameRes/Prefabs/IceBreaker/IceBreakerPlayer.prefab";
            public const string Level_File = "Assets/GameRes/Prefabs/IceBreaker/Level";
            public const string LevelGenerator_Prefab = "Assets/GameRes/Prefabs/IceBreaker/LevelGenerator.prefab";
            public const string PlayerShard_Prefab = "Assets/GameRes/Prefabs/IceBreaker/PlayerShard.prefab";
            public static class Level
            {
                public const string EnemyPrefab_Prefab = "Assets/GameRes/Prefabs/IceBreaker/Level/EnemyPrefab.prefab";
                public const string EnemyShard_Prefab = "Assets/GameRes/Prefabs/IceBreaker/Level/EnemyShard.prefab";
                public const string GoalPrefab_Prefab = "Assets/GameRes/Prefabs/IceBreaker/Level/GoalPrefab.prefab";
                public const string ObstaclePrefab_Prefab = "Assets/GameRes/Prefabs/IceBreaker/Level/ObstaclePrefab.prefab";
                public const string PlatformPrefab_Prefab = "Assets/GameRes/Prefabs/IceBreaker/Level/PlatformPrefab.prefab";
                public enum LevelName
                {
                    EnemyPrefab,
                    EnemyShard,
                    GoalPrefab,
                    ObstaclePrefab,
                    PlatformPrefab
                }
                public static string GetLevel(string path) => "Assets/GameRes/Prefabs/IceBreaker/Level/" + path + ".prefab";
            }
            public enum IceBreakerName
            {
                IceBreakerCamera,
                IceBreakerManager,
                IceBreakerPlayer,
                Level,
                LevelGenerator,
                PlayerShard
            }
            public static string GetIceBreaker(string path) => "Assets/GameRes/Prefabs/IceBreaker/" + path + ".prefab";
        }
        public static class TriggerDialogueBubble
        {
            public const string TriggerDialogueBubble0_Prefab = "Assets/GameRes/Prefabs/TriggerDialogueBubble/TriggerDialogueBubble0.prefab";
            public const string TriggerDialogueBubble1_Prefab = "Assets/GameRes/Prefabs/TriggerDialogueBubble/TriggerDialogueBubble1.prefab";
            public const string TriggerDialogueBubble2_Prefab = "Assets/GameRes/Prefabs/TriggerDialogueBubble/TriggerDialogueBubble2.prefab";
            public enum TriggerDialogueBubbleName
            {
                TriggerDialogueBubble0,
                TriggerDialogueBubble1,
                TriggerDialogueBubble2
            }
            public static string GetTriggerDialogueBubble(string path) => "Assets/GameRes/Prefabs/TriggerDialogueBubble/" + path + ".prefab";
        }
        public static class UI
        {
            public const string GlitchEffect_Prefab = "Assets/GameRes/Prefabs/UI/GlitchEffect.prefab";
            public const string LoadingScreenDefault_Prefab = "Assets/GameRes/Prefabs/UI/LoadingScreenDefault.prefab";
            public const string MainMenu_File = "Assets/GameRes/Prefabs/UI/MainMenu";
            public const string Playthrough1_File = "Assets/GameRes/Prefabs/UI/Playthrough1";
            public const string Playthrough2_File = "Assets/GameRes/Prefabs/UI/Playthrough2";
            public const string Playthrough3_File = "Assets/GameRes/Prefabs/UI/Playthrough3";
            public static class MainMenu
            {
                public const string MainMenuForm_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/MainMenuForm.prefab";
                public const string Window_File = "Assets/GameRes/Prefabs/UI/MainMenu/Window";
                public static class Window
                {
                    public const string AdWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/AdWindow.prefab";
                    public const string AudioWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/AudioWindow.prefab";
                    public const string ConfirmCrackWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/ConfirmCrackWindow.prefab";
                    public const string CreditWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/CreditWindow.prefab";
                    public const string DownloadIcebreakerWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/DownloadIcebreakerWindow.prefab";
                    public const string DownloadTLHWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/DownloadTLHWindow.prefab";
                    public const string EmptyWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/EmptyWindow.prefab";
                    public const string FutureUpdatePlanWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/FutureUpdatePlanWindow.prefab";
                    public const string IceBreakerPreWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/IceBreakerPreWindow.prefab";
                    public const string QuitWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/QuitWindow.prefab";
                    public const string RubbishCanWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/RubbishCanWindow.prefab";
                    public const string SettingWindow_Prefab = "Assets/GameRes/Prefabs/UI/MainMenu/Window/SettingWindow.prefab";
                    public enum WindowName
                    {
                        AdWindow,
                        AudioWindow,
                        ConfirmCrackWindow,
                        CreditWindow,
                        DownloadIcebreakerWindow,
                        DownloadTLHWindow,
                        EmptyWindow,
                        FutureUpdatePlanWindow,
                        IceBreakerPreWindow,
                        QuitWindow,
                        RubbishCanWindow,
                        SettingWindow
                    }
                    public static string GetWindow(string path) => "Assets/GameRes/Prefabs/UI/MainMenu/Window/" + path + ".prefab";
                }
                public enum MainMenuName
                {
                    MainMenuForm,
                    Window
                }
                public static string GetMainMenu(string path) => "Assets/GameRes/Prefabs/UI/MainMenu/" + path + ".prefab";
            }
            public static class Playthrough1
            {
                public const string Choice_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/Choice.prefab";
                public const string DialogueViewType_File = "Assets/GameRes/Prefabs/UI/Playthrough1/DialogueViewType";
                public const string GlitchWindow_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/GlitchWindow.prefab";
                public const string LoadingScreenPlayThrough1_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/LoadingScreenPlayThrough1.prefab";
                public const string Manager_File = "Assets/GameRes/Prefabs/UI/Playthrough1/Manager";
                public const string Maps_File = "Assets/GameRes/Prefabs/UI/Playthrough1/Maps";
                public const string PauseForm_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/PauseForm.prefab";
                public const string SkipButton_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/SkipButton.prefab";
                public const string TouchControls_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/TouchControls.prefab";
                public static class DialogueViewType
                {
                    public const string BubbleView_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/DialogueViewType/BubbleView.prefab";
                    public const string NarratorView_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/DialogueViewType/NarratorView.prefab";
                    public const string TraditionalView_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/DialogueViewType/TraditionalView.prefab";
                    public enum DialogueViewTypeName
                    {
                        BubbleView,
                        NarratorView,
                        TraditionalView
                    }
                    public static string GetDialogueViewType(string path) => "Assets/GameRes/Prefabs/UI/Playthrough1/DialogueViewType/" + path + ".prefab";
                }
                public static class Manager
                {
                    public const string UIManager1_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/Manager/UIManager1.prefab";
                    public enum ManagerName
                    {
                        UIManager1
                    }
                    public static string GetManager(string path) => "Assets/GameRes/Prefabs/UI/Playthrough1/Manager/" + path + ".prefab";
                }
                public static class Maps
                {
                    public const string MapPanel0_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough1/Maps/MapPanel0.prefab";
                    public enum MapsName
                    {
                        MapPanel0
                    }
                    public static string GetMaps(string path) => "Assets/GameRes/Prefabs/UI/Playthrough1/Maps/" + path + ".prefab";
                }
                public enum Playthrough1Name
                {
                    Choice,
                    DialogueViewType,
                    GlitchWindow,
                    LoadingScreenPlayThrough1,
                    Manager,
                    Maps,
                    PauseForm,
                    SkipButton,
                    TouchControls
                }
                public static string GetPlaythrough1(string path) => "Assets/GameRes/Prefabs/UI/Playthrough1/" + path + ".prefab";
            }
            public static class Playthrough2
            {
                public const string ChoiceButton_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough2/ChoiceButton.prefab";
                public const string LoadingScreenPlayThrough2_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough2/LoadingScreenPlayThrough2.prefab";
                public const string SkipButton_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough2/SkipButton.prefab";
                public enum Playthrough2Name
                {
                    ChoiceButton,
                    LoadingScreenPlayThrough2,
                    SkipButton
                }
                public static string GetPlaythrough2(string path) => "Assets/GameRes/Prefabs/UI/Playthrough2/" + path + ".prefab";
            }
            public static class Playthrough3
            {
                public const string LoadingScreenPlayThrough3_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough3/LoadingScreenPlayThrough3.prefab";
                public const string LoginForm_Prefab = "Assets/GameRes/Prefabs/UI/Playthrough3/LoginForm.prefab";
                public enum Playthrough3Name
                {
                    LoadingScreenPlayThrough3,
                    LoginForm
                }
                public static string GetPlaythrough3(string path) => "Assets/GameRes/Prefabs/UI/Playthrough3/" + path + ".prefab";
            }
            public enum UIName
            {
                GlitchEffect,
                LoadingScreenDefault,
                MainMenu,
                Playthrough1,
                Playthrough2,
                Playthrough3
            }
            public static string GetUI(string path) => "Assets/GameRes/Prefabs/UI/" + path + ".prefab";
        }
        public static class Video
        {
            public const string VideoManager_Prefab = "Assets/GameRes/Prefabs/Video/VideoManager.prefab";
            public enum VideoName
            {
                VideoManager
            }
            public static string GetVideo(string path) => "Assets/GameRes/Prefabs/Video/" + path + ".prefab";
        }
        public static class World
        {
            public const string CallNarrator0_Prefab = "Assets/GameRes/Prefabs/World/CallNarrator0.prefab";
            public const string MaodieVillage_Prefab = "Assets/GameRes/Prefabs/World/MaodieVillage.prefab";
            public const string ToiletVillage2Platform_Prefab = "Assets/GameRes/Prefabs/World/ToiletVillage2Platform.prefab";
            public const string ToiletVillagePlatform_Prefab = "Assets/GameRes/Prefabs/World/ToiletVillagePlatform.prefab";
            public enum WorldName
            {
                CallNarrator0,
                MaodieVillage,
                ToiletVillage2Platform,
                ToiletVillagePlatform
            }
            public static string GetWorld(string path) => "Assets/GameRes/Prefabs/World/" + path + ".prefab";
        }
        public enum PrefabsName
        {
            Camera,
            Character,
            Dialogue,
            DialogueBubbles,
            Effect,
            Empty,
            Enemy,
            Environment,
            IceBreaker,
            TriggerDialogueBubble,
            UI,
            Video,
            World
        }
        public static string GetPrefabs(string path) => "Assets/GameRes/Prefabs/" + path + "";
    }
    public static class Scenes
    {
        public const string IceBreaker_Unity = "Assets/GameRes/Scenes/IceBreaker.unity";
        public const string MainMenu_Unity = "Assets/GameRes/Scenes/MainMenu.unity";
        public const string OpenVideo1_Unity = "Assets/GameRes/Scenes/OpenVideo1.unity";
        public const string OpenVideo2_Unity = "Assets/GameRes/Scenes/OpenVideo2.unity";
        public const string Playthrough3_File = "Assets/GameRes/Scenes/Playthrough3";
        public const string Test_Unity = "Assets/GameRes/Scenes/Test.unity";
        public const string ToiletVillage_Unity = "Assets/GameRes/Scenes/ToiletVillage.unity";
        public static class Playthrough3
        {
            public const string Login_Unity = "Assets/GameRes/Scenes/Playthrough3/Login.unity";
            public const string Menu_Unity = "Assets/GameRes/Scenes/Playthrough3/Menu.unity";
            public enum Playthrough3Name
            {
                Login,
                Menu
            }
            public static string GetPlaythrough3(string path) => "Assets/GameRes/Scenes/Playthrough3/" + path + ".unity";
        }
        public enum ScenesName
        {
            IceBreaker,
            MainMenu,
            OpenVideo1,
            OpenVideo2,
            Playthrough3,
            Test,
            ToiletVillage
        }
        public static string GetScenes(string path) => "Assets/GameRes/Scenes/" + path + ".unity";
    }
    public static class ScriptableObjects
    {
        public const string Effect_File = "Assets/GameRes/ScriptableObjects/Effect";
        public const string Inventory_File = "Assets/GameRes/ScriptableObjects/Inventory";
        public const string Quest_File = "Assets/GameRes/ScriptableObjects/Quest";
        public static class Effect
        {
            public const string Shakes_Asset = "Assets/GameRes/ScriptableObjects/Effect/Shakes.asset";
            public enum EffectName
            {
                Shakes
            }
            public static string GetEffect(string path) => "Assets/GameRes/ScriptableObjects/Effect/" + path + ".asset";
        }
        public static class Inventory
        {
            public const string Item_Inventory_SO_Asset = "Assets/GameRes/ScriptableObjects/Inventory/Item Inventory SO.asset";
            public enum InventoryName
            {
                Item_Inventory_SO
            }
            public static string GetInventory(string path) => "Assets/GameRes/ScriptableObjects/Inventory/" + path + ".asset";
        }
        public static class Quest
        {
            public const string Complain_About_Optimization_Asset = "Assets/GameRes/ScriptableObjects/Quest/Complain About Optimization.asset";
            public const string Destroy_Bugs_Asset = "Assets/GameRes/ScriptableObjects/Quest/Destroy Bugs.asset";
            public enum QuestName
            {
                Complain_About_Optimization,
                Destroy_Bugs
            }
            public static string GetQuest(string path) => "Assets/GameRes/ScriptableObjects/Quest/" + path + ".asset";
        }
        public enum ScriptableObjectsName
        {
            Effect,
            Inventory,
            Quest
        }
        public static string GetScriptableObjects(string path) => "Assets/GameRes/ScriptableObjects/" + path + "";
    }
    public enum GameResName
    {
        Animations,
        Art,
        Audio,
        Data,
        Prefabs,
        Scenes,
        ScriptableObjects
    }
    public static string GetGameRes(string path) => "Assets/GameRes/" + path + "";
}
}
