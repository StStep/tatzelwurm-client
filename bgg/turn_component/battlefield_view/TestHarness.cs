using Godot;
using System;
using System.Collections.Generic;

public class TestHarness : Node
{
    BattlefieldView _battlefieldViewNode;
    Timer _testStepClk;

    public override void _Ready()
    {
        _battlefieldViewNode = GetNode<BattlefieldView>("BattlefieldView");
        _testStepClk = GetNode<Timer>("TestStepClk");
        SetProcess(true);
    }


    // Testing mutliple annoations at once
    private IEnumerable<int> Test1()
    {
        var i = 0;
        GD.Print("Running Test One...");

        // Clear
        _battlefieldViewNode.clear();
        yield return i++;

        // Add Unit
        var uid = _battlefieldViewNode.new_unit(new Vector2(200,200), new Vector2(1,1));
        yield return i++;

        // Add Generic Command 1
        _battlefieldViewNode.AddMove(uid, false, new Vector2(330,330));
        yield return i++;

        // Add Generic Command 2
        _battlefieldViewNode.AddMove(uid, false, new Vector2(270,390), new Vector2(0.5f,1), new List<String>() {"reposition", "rotation"});
        yield return i++;

        GD.Print("Completed Test One");
    }


    // Test rotation command annotation and invalid highlighting
    private IEnumerable<int> Test2()
    {
        var i = 0;
        GD.Print("Running Test Two...");

        // Clear
        _battlefieldViewNode.clear();
        yield return i++;

        // Add Unit
        var uid = _battlefieldViewNode.new_unit(new Vector2(200,200), new Vector2(1,1));
        _battlefieldViewNode.display_eot_move(uid, true);
        yield return i++;

        // Add Generic Command 1
        _battlefieldViewNode.AddMove(uid, false, new Vector2(945,410), new  Vector2(-130,-130), new List<String>() {"rotation"});
        yield return i++;

        // TODO: Color Command 1and last half of path as Invalid
        //_battlefieldViewNode.highlight_cmd_path(unit_ref, 1, "Invalid", .5)
        //_battlefieldViewNode.highlight_cmd_body(unit_ref, 1, "Invalid");
        yield return i++;

        // Add Generic Command 2
        _battlefieldViewNode.AddMove(uid, false, new Vector2(815,280));
        yield return i++;

        GD.Print("Completed Test Two");
    }

    // Test Wheel and Reposition commands
    private IEnumerable<int> Test3()
    {
        var i = 0;
        GD.Print("Running Test Three...");

        // Clear
        _battlefieldViewNode.clear();
        yield return i++;

        // Add Unit
        var uid = _battlefieldViewNode.new_unit(new Vector2(200,200), new Vector2(1,1));
        _battlefieldViewNode.display_eot_move(uid, true);
        yield return i++;

        // TODO: Color Unit as focused
        //_battlefieldViewNode.highlight_cmd_path(unit_ref, 1, "Focus", Vector2(.5, 1))
        //_battlefieldViewNode.highlight_cmd_body(unit_ref, 0, "Focus");
        yield return i++;

        // Add Generic Command 1
        _battlefieldViewNode.AddMove(uid, false, new Vector2(330,330));
        yield return i++;

        // Add Generic Command 2
        _battlefieldViewNode.AddMove(uid, false, new Vector2(335,340), new List<String>() {"wheel"});
        yield return i++;

        // Add Generic Command 3
        _battlefieldViewNode.AddMove(uid, false, new Vector2(385,240), new Vector2(5,10), new List<String>() {"reposition"});
        yield return i++;

        // Add Generic Command 4
        _battlefieldViewNode.AddMove(uid, false, new Vector2(395,245), new List<String>() {"wheel"});
        yield return i++;

        GD.Print("Completed Test Three");
    }

    // Test drawing an arc
    private IEnumerable<int> Test4()
    {
        var i = 0;
        GD.Print("Running Test Four...");

        // Clear
        _battlefieldViewNode.clear();
        yield return i++;

        // Add Unit
        var uid = _battlefieldViewNode.new_unit(new Vector2(200,200), new Vector2(1,1));
        _battlefieldViewNode.display_eot_move(uid, true);
        yield return i++;

        // Add Generic Command 1
        _battlefieldViewNode.AddMove(uid, false, new Vector2(330,330));
        yield return i++;

        // Add Generic Command 2
        _battlefieldViewNode.AddMove(uid, true, new Vector2(460, 370));
        yield return i++;

        GD.Print("Completed Test Four");
    }

    async private void RunTest(Func<IEnumerable<int>> test)
    {
        if (!_testStepClk.IsStopped())
        {
            GD.Print("Wait for previous test to complete");
        }
        else
        {
            _testStepClk.Start();
            foreach (var i in test())
            {
                GD.Print("Step " + i);
                await ToSignal(_testStepClk, "timeout");
            }
            _testStepClk.Stop();
        }
    }

    private void test_1() => RunTest(Test1);

    private void test_2() => RunTest(Test2);

    private void test_3() => RunTest(Test3);

    private void test_4() => RunTest(Test4);
}