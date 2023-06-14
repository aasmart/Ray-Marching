using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[ExecuteInEditMode]
public class Animator : MonoBehaviour {
    public Vector3 Rotation;
    public string XExpression = "";
    public string YExpression = "";
    public string ZExpression = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update() {
        var trans = transform;

        trans.Rotate(Rotation * Time.deltaTime);

        var translate = new Vector3(
            Evaluate(XExpression) * Time.deltaTime,
            Evaluate(YExpression) * Time.deltaTime,
            Evaluate(ZExpression) * Time.deltaTime
        );

        trans.position += translate;
    }
    
    private float Evaluate(string expression) {
        if (expression.Length <= 0)
            return 0;
        
        var replacedExpression = 
            expression.Replace("time", Time.time.ToString(CultureInfo.InvariantCulture));

        return !ExpressionEvaluator.Evaluate(replacedExpression, out float result) ? 0 : result;
    }
}
