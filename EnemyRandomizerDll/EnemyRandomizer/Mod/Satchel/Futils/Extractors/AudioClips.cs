using HutongGames.PlayMaker.Actions;
using nv;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
The MIT License (MIT)

Copyright (c) 2018 Yoshifumi Kawai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
//MIT Lisc: Original src taken from: https://github.com/PrashantMohta/Satchel/ 

namespace nv.Futils.Extractors
{
    public static class AudioClips{
        /// <summary>
        /// Extract all Audio Clips on a PlayMakerFSM
        /// </summary>
        /// <param name="self">The PlayMakerFSM</param>
        /// <returns></returns>
        public static Dictionary<string,AudioClip> GetAudioClips(this PlayMakerFSM self){
            Dictionary<string,AudioClip> ac = new Dictionary<string,AudioClip>();
            foreach (var state in self.FsmStates)
            {
                foreach(var action in state.Actions){
                    Type t = action.GetType();
                    if (action is AudioPlay){
                        AudioPlay a = (AudioPlay) action;
                        if(a.oneShotClip == null){ 
                            Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.oneShotClip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } else if(action is AudioPlayerOneShot){
                        AudioPlayerOneShot a = (AudioPlayerOneShot) action;
                        foreach( var clip in a.audioClips){
                            if(clip == null){ 
                               Dev.LogError($"clip null in action {action.Name}");
                                continue; 
                            }
                            ac[clip.name]=clip;
                        }
                    }  else if(action is AudioPlayerOneShotSingle){
                        AudioPlayerOneShotSingle a = (AudioPlayerOneShotSingle) action;
                        if(a.audioClip == null){ 
                           Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.audioClip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } else if (action is AudioPlayRandom){
                        AudioPlayRandom a = (AudioPlayRandom) action;
                        foreach( var clip in a.audioClips){
                            if(clip == null){ 
                               Dev.LogError($"clip null in action {action.Name}");
                                continue; 
                            }
                            ac[clip.name]=clip;
                        }
                    } else if (action is AudioPlayRandomSingle){
                        AudioPlayRandomSingle a = (AudioPlayRandomSingle) action;
                        if(a.audioClip == null){ 
                           Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.audioClip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } else if (action is AudioPlaySimple){
                        AudioPlaySimple a = (AudioPlaySimple) action;
                        if(a.oneShotClip == null){ 
                           Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.oneShotClip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } else if (action is AudioPlayV2){
                        AudioPlayV2 a = (AudioPlayV2) action;
                        if(a.oneShotClip == null){ 
                           Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.oneShotClip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } else if (action is PlayRandomSound){
                        PlayRandomSound a = (PlayRandomSound) action;
                        foreach( var clip in a.audioClips){
                            if(clip == null){ 
                               Dev.LogError($"clip null in action {action.Name}");
                                continue; 
                            }
                            ac[clip.name]=clip;
                        }
                    } else if (action is PlaySound){
                        PlaySound a = (PlaySound) action;
                        if(a.clip == null){ 
                           Dev.LogError($"clip null in action {action.Name}");
                            continue; 
                        }
                        var clip = a.clip.Value as AudioClip;
                        ac[clip.name]=clip;
                    } 
                }
            }
            return ac;
        }
    
    }
}