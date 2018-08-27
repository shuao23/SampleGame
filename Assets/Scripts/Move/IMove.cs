﻿public interface IMove
{
    bool Issued { get; }
    bool InRightCondition { get; }
    string Name { get; }

    void Issue(bool doOverride = false);
    void Close();
    bool TryUpdate(float deltaTime);
}