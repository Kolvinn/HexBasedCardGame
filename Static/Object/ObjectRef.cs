
using System;


[Serializable]
public class ObjectRef{

    public object storedReference;
    public ObjectRef(object reff){
        this.storedReference = reff;
    }
}