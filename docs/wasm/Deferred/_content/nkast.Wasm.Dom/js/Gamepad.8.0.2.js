window.nkGamepad =
{
    GetVibrationActuator: function(uid)
    {
        var gp = nkJSObject.GetObject(uid);
        var ha = gp.vibrationActuator;

        var uid = nkJSObject.objectMap.indexOf(ha);
        if (uid !== -1)
            return (uid+1);

        return nkJSObject.RegisterObject(ha);
    },
    GetId: function (uid)
    {
        var gp = nkJSObject.GetObject(uid);
        return BINDING.js_to_mono_obj(gp.id);
    },
    GetIndex: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        return gp.index;
    },
    GetConnected: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        return gp.connected;
    },
    GetMapping: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        return BINDING.js_to_mono_obj(gp.mapping);
    },
    GetTimestamp: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        return Math.floor(gp.timestamp*1000);
    },
    GetButtons: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        var buttons = gp.buttons;
        var btns = [];
        for (var i = 0; i < buttons.length; i++)
        {
            btns[i*3+0] = buttons[i].value;
            btns[i*3+1] = (buttons[i].pressed) ? 1 : 0;
            if (buttons[i].touched2 == undefined)
                btns[i*3+2] = 0;
            else
                btns[i*3+2] = (buttons[i].touched) ? 1 : 0;
        }
        var str = btns.toString();

        return BINDING.js_to_mono_obj(str);
    },
    GetAxes: function (uid, d)
    {
        var gp = nkJSObject.GetObject(uid);
        var axes = gp.axes;
        var str = axes.toString();

        return BINDING.js_to_mono_obj(str);
    }
};

window.nkGamepadHapticActuator =
{
    PlayEffect: function (uid, d)
    {
        var ha = nkJSObject.GetObject(uid);
        var sd = Blazor.platform.readFloatField(d, 0);
        var du = Blazor.platform.readFloatField(d, 4);
        var sm = Blazor.platform.readFloatField(d, 8);
        var wm = Blazor.platform.readFloatField(d,12);
        var lt = Blazor.platform.readFloatField(d,16);
        var rt = Blazor.platform.readFloatField(d,20);

        if ('playEffect' in ha)
        {
            ha.playEffect("dual-rumble",
            {
                startDelay: sd,
                duration: du,
                strongMagnitude: sm,
                weakMagnitude: wm,
                leftTrigger: lt,
                rightTrigger: rt
            });
            return true;
        }
        return false;
    },
    Pulse: function (uid, d)
    {
        var ha = nkJSObject.GetObject(uid);
        var va = Blazor.platform.readFloatField(d, 0);
        var du = Blazor.platform.readFloatField(d, 4);

        if ('pulse' in ha)
        {
            ha.pulse(va, du);
            return true;
        }
        return false;
    },
    Reset: function (uid, d)
    {
        var ha = nkJSObject.GetObject(uid);

        if ('reset' in ha)
        {
            ha.reset();
            return true;
        }
        return false;
    },
};


