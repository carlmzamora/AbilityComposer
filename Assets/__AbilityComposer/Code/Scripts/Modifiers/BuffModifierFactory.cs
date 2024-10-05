using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffModifierFactory : ModifierFactory<BuffModifier>
{
    private void Awake()
    {
        modifierStats.Add(new FloatAttribute(ModifierKeys.BUFF_DURATION));
    }
}

public class BuffModifier : Modifier
{

}
