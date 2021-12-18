
using System;
using Newtonsoft.Json.Converters;

public class TestModel{
    
    private string ANotherName = "something or rather";
    public TestModel(){

    }

    public string GetAnotherName(){
        return this.ANotherName;
    }
}