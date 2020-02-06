using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MyDevices: ScriptableObject{
    public class AdbDevice{
        public string name;
        public string macAdress;
    }

    public List<AdbDevice> devices;

}