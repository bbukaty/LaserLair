using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct intTrio {
    public int x;
    public int y;
    public int z;

    public intTrio(int xIn, int yIn, int zIn) {
        x = xIn;
        y = yIn;
        z = zIn;
    }
    public intTrio(Vector3 v) {
        for (int i = 0; i < 3; i++) {
            Debug.Assert(Mathf.Abs((float)(int)v[i] - v[i]) < 0.001, "Warning: Level contains improperly placed cube!");
        }
        x = (int)v[0];
        y = (int)v[1];
        z = (int)v[2];
    }

    public int this[int index] { 
        get {
            switch(index) {
                case 0:
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                default:
                    Debug.LogError("Error: invalid index into intTrio struct!");
                    return 0;
            }
        }
        set {
            switch(index) {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    Debug.LogError("Error: invalid index into intTrio struct!");
                    break;
            }
        }
    }


    public static bool operator== (intTrio a, intTrio b) {
        return (a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static bool operator!= (intTrio a, intTrio b) {
        return !(a == b);
    }

    // it kept warning me to put these functions in the struct
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }
        
        return this == (intTrio) obj;
    }
    
    // really barebones, should never be used - just quieting a warning
    public override int GetHashCode()
    {
        return x*100 + y*10 + z;
    }
    
    public static intTrio operator+ (intTrio a, intTrio b) {
        return new intTrio(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static intTrio operator* (intTrio a, int s) {
        return new intTrio(a.x * s, a.y * s, a.z * s);
    }
    public static intTrio operator* (int s, intTrio a) {
        return a * s;
    }

    public override string ToString() {
        return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
    }

}