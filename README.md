# RTask
[![License](https://img.shields.io/github/license/mistletoeKANO/RTask)]([https://github.com/mistletoeKANO/RTask/blob/master/LICENSE](https://github.com/mistletoeKANO/RTask/blob/main/LICENSE))

## A simple efficient mainThread allocation free async/await integration for Unity.

1.Struct based RTask<T> and custom AsyncMethodBuilder to achieve zero allocation
2.Makes all Unity AsyncOperations and Coroutines awaitable
3.PlayerLoop based task(RTask.Delay, RTask.DelayFrame, RTask.DelayAction etc..) that enable replacing all coroutine operations

## Simple Example

``
private async void TestRTask()
{
    //milliseconds, use unscaledDealtTime
    await RTask.RTask.Delay(1000, true);

    await RTask.RTask.DelayFrame(10);

    await RTask.RTask.NextFrame();

    await RTask.RTask.EndOfFrame();

    await RTask.RTask.DelayFrame(0);
}

private void TestAction()
{
    RTask.RTask.DelayFrameAction(111, () =>
    {
        Debug.Log("DelayFrameAction");
    });
    //milliseconds
    RTask.RTask.DelayAction(2000, () =>
    {
        Debug.Log("DelayAction");
    });
}
``
