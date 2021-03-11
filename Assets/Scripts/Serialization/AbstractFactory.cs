using System.Collections.Generic;
using MongoDB.Bson;

using Data;
using Data.MOF;
using DG;
using UnityEngine;

public abstract class AbstractFactory : MonoBehaviour
{
    public abstract void RegisterFactory(FactoryRegister register);

    public virtual void Start()
    {
        RegisterFactory(GameObject.FindObjectOfType<FactoryRegister>());
    }

    // SAVE
    public abstract List<MofElement> BuildMofFromDg(DgElement dg, XmiCollection container);
    public abstract void UpdateMofFromDg(DgElement dg, XmiCollection container);

    // LOAD
    public abstract List<MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container);
    public abstract void UpdateMofFromJson(BsonDocument json, XmiCollection container);
    public abstract List<DgElement> BuilDgFromMof(MofElement mof, XmiCollection container);
    public abstract void UpdateDgFromMof(DgElement dg, XmiCollection container);






}
