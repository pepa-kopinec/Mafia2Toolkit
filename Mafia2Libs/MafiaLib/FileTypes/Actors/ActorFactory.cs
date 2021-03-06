﻿using Gibbed.Illusion.FileFormats.Hashing;
using System;
using System.IO;

namespace ResourceTypes.Actors
{
    public static class ActorFactory
    {
        public static IActorExtraDataInterface CreateExtraData(ActorTypes type)
        {
            switch(type)
            {
                case ActorTypes.Human:
                    return new ActorHuman();
                case ActorTypes.C_TrafficCar:
                    return new ActorTrafficCar();
                case ActorTypes.C_TrafficHuman:
                    return new ActorTrafficHuman();
                case ActorTypes.C_TrafficTrain:
                    return new ActorTrafficTrain();
                case ActorTypes.C_Item:
                    return new ActorItem();
                case ActorTypes.C_Door:
                    return new ActorDoor();
                case ActorTypes.C_Sound:
                    return new ActorSoundEntity();
                case ActorTypes.StaticEntity:
                    return new ActorStaticEntity();
                case ActorTypes.FrameWrapper:
                    return new ActorFrameWrapper();
                case ActorTypes.C_ActorDetector:
                    return new ActorActorDetector();
                case ActorTypes.C_StaticParticle:
                    return new ActorStaticParticle();
                case ActorTypes.LightEntity:
                    return new ActorLight();
                case ActorTypes.C_Cutscene:
                    throw new NotImplementedException();
                case ActorTypes.C_ScriptEntity:
                    return new ActorScriptEntity();
                case ActorTypes.C_Pinup:
                    return new ActorPinup();
                default:
                    throw new NotImplementedException();
            }
        }

        public static IActorExtraDataInterface LoadExtraData(ActorTypes type, MemoryStream stream, bool isBigEndian)
        {
            switch(type)
            {
                case ActorTypes.Human:
                    return new ActorHuman(stream, isBigEndian);
                case ActorTypes.C_TrafficCar:
                    return new ActorTrafficCar(stream, isBigEndian);
                case ActorTypes.C_TrafficHuman:
                    return new ActorTrafficHuman(stream, isBigEndian);
                case ActorTypes.C_TrafficTrain:
                    return new ActorTrafficTrain(stream, isBigEndian);
                case ActorTypes.C_Item:
                    return new ActorItem(stream, isBigEndian);
                case ActorTypes.C_Door:
                    return new ActorDoor(stream, isBigEndian);
                case ActorTypes.C_Sound:
                    return new ActorSoundEntity(stream, isBigEndian);
                case ActorTypes.Radio:
                    return new ActorRadio(stream, isBigEndian);
                case ActorTypes.StaticEntity:
                    return new ActorStaticEntity(stream, isBigEndian);
                case ActorTypes.FrameWrapper:
                    return new ActorFrameWrapper(stream, isBigEndian);
                case ActorTypes.C_ActorDetector:
                    return new ActorActorDetector(stream, isBigEndian);
                case ActorTypes.C_StaticParticle:
                    return new ActorStaticParticle(stream, isBigEndian);
                case ActorTypes.LightEntity:
                    return new ActorLight(stream, isBigEndian);
                case ActorTypes.C_ScriptEntity:
                    return new ActorScriptEntity(stream, isBigEndian);
                case ActorTypes.C_Pinup:
                    return new ActorPinup(stream, isBigEndian);
                case ActorTypes.SpikeStrip:
                    return new ActorSpikeStrip(stream, isBigEndian);
                case ActorTypes.Wardrobe:
                    return new ActorWardrobe(stream, isBigEndian);
                case ActorTypes.CleanEntity:
                    return new ActorCleanEntity(stream, isBigEndian);
                default:
                    return null;
            }
        }

        public static ActorEntry CreateActorItem(ActorTypes type, string name)
        {
            ActorEntry item = new ActorEntry();
            item.ActorTypeID = (int)type;
            item.ActorTypeName = type.ToString();
            item.EntityName = name;
            item.EntityHash = FNV64.Hash(item.EntityName);
            return item;
        }
    }
}
