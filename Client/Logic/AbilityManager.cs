// using System;
//
// namespace Client.Logic;
//
// public class AbilityManager(Turtle turtle)
// {
//     private DateTime _lastToggleTime = DateTime.MinValue;
//     private DateTime _invisibleUntil = DateTime.MinValue;
//
//     private const float InvisibleDuration = 3f;
//     private const float Cooldown = 5f;
//
//     public void Update()
//     {
//         if (turtle.IsVisible || DateTime.Now < _invisibleUntil) return;
//         turtle.SetVisible(true);
//         _lastToggleTime = DateTime.Now;
//     }
//
//     public void ToggleInvisibility()
//     {
//         var now = DateTime.Now;
//
//         if (turtle.IsVisible)
//         {
//             if ((now - _lastToggleTime).TotalSeconds < Cooldown)
//                 return;
//
//             turtle.SetVisible(false);
//             _lastToggleTime = now;
//             _invisibleUntil = now.AddSeconds(InvisibleDuration);
//         }
//         else
//         {
//             turtle.SetVisible(true);
//             _lastToggleTime = now;
//         }
//     }
// }
