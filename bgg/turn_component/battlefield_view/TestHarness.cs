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

/*
// Test rotation command annotation and invalid highlighting
func _test_2():
    print("Running Test Two...")
    var unit_ref = "unit_2"

    // Clear
    _battlefieldViewNode.clear()
    yield()

    // Add Unit
    _battlefieldViewNode.new_unit(unit_ref, Vector2(815,541), Vector2(1,-1))
    _battlefieldViewNode.display_eot_move(unit_ref, true)
    yield()

    // Add Generic Command 1
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(945,410),
            {"annotation" : "rotation", "end_gdir" : Vector2(-130,-130)})
    yield()

    // Color Command 1and last half of path as Invalid
    _battlefieldViewNode.highlight_cmd_path(unit_ref, 1, "Invalid", .5)
    _battlefieldViewNode.highlight_cmd_body(unit_ref, 1, "Invalid")

    // Add Generic Command 2
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(815,280))
    yield()

    print("Completed Test Two")
    return null

// Test Wheel and Reposition commands
func _test_3():
    print("Running Test Three...")
    var unit_ref = "unit_3"

    // Clear
    _battlefieldViewNode.clear()
    yield()

    // Add Unit
    _battlefieldViewNode.new_unit(unit_ref, Vector2(200,200), Vector2(1,1))
    _battlefieldViewNode.display_eot_move(unit_ref, true)
    yield()

    // Color Unit as focused
    _battlefieldViewNode.highlight_cmd_path(unit_ref, 1, "Focus", Vector2(.5, 1))
    _battlefieldViewNode.highlight_cmd_body(unit_ref, 0, "Focus")

    // Add Generic Command 1
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(330,330))
    yield()

    // Add Generic Command 2
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(335,340), {"annotation" : "wheel"})
    yield()

    // Add Generic Command 3
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(385,240),
            {"annotation" : "reposition", "end_gdir" : Vector2(5,10)})
    yield()

    // Add Generic Command 4
    _battlefieldViewNode.add_cmd(unit_ref, Vector2(395,245), {"annotation" : "wheel"})
    yield()

    print("Completed Test Three")
    return null
*/

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

    private void test_2() => GD.Print("Not implemented");

    private void test_3() => GD.Print("Not implemented");

    private void test_4() => RunTest(Test4);
}