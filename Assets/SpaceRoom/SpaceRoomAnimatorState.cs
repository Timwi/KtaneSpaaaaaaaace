using System.Collections;
using UnityEngine;

public class SpaceRoomAnimatorState : StateMachineBehaviour
{
    private static readonly int HOLDABLE_PARAMETER = Animator.StringToHash("Holdable");
    private static readonly int HOLDABLE_MERGE_PARAMETER = Animator.StringToHash("HoldableMerge");
    private static readonly int FORWARD_MERGE_PARAMETER = Animator.StringToHash("ForwardMerge");

    public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

    private int _oldHoldableValue = 0;
    private IEnumerator _holdableCoroutine = null;
    private IEnumerator _forwardCoroutine = null;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        int holdableValue = animator.GetInteger(HOLDABLE_PARAMETER);
        if (holdableValue != _oldHoldableValue)
        {
            SpaceRoomAnimator spaceRoomAnimator = animator.GetComponent<SpaceRoomAnimator>();

            switch (holdableValue)
            {
                case 0:
                    if (_forwardCoroutine != null)
                    {
                        spaceRoomAnimator.StopCoroutine(_forwardCoroutine);                        
                    }
                    _forwardCoroutine = MovementCoroutine(animator, FORWARD_MERGE_PARAMETER, 0.0f, 0.6f);
                    spaceRoomAnimator.StartCoroutine(_forwardCoroutine);
                    break;

                default:
                    if (_oldHoldableValue == 0)
                    {
                        if (_forwardCoroutine != null)
                        {
                            spaceRoomAnimator.StopCoroutine(_forwardCoroutine);
                        }
                        _forwardCoroutine = MovementCoroutine(animator, FORWARD_MERGE_PARAMETER, 1.0f, 0.6f);
                        spaceRoomAnimator.StartCoroutine(_forwardCoroutine);

                        if (_holdableCoroutine == null)
                        {
                            animator.SetFloat(HOLDABLE_MERGE_PARAMETER, holdableValue);
                        }
                        else
                        {
                            spaceRoomAnimator.StopCoroutine(_holdableCoroutine);
                            _holdableCoroutine = MovementCoroutine(animator, HOLDABLE_MERGE_PARAMETER, holdableValue, 0.6f);
                            spaceRoomAnimator.StartCoroutine(_holdableCoroutine);
                        }
                    }
                    else
                    {
                        if (_holdableCoroutine != null)
                        {
                            spaceRoomAnimator.StopCoroutine(_holdableCoroutine);
                        }
                        _holdableCoroutine = MovementCoroutine(animator, HOLDABLE_MERGE_PARAMETER, holdableValue, 0.6f);
                        spaceRoomAnimator.StartCoroutine(_holdableCoroutine);
                    }
                    break;
            }
        }

        _oldHoldableValue = holdableValue;
    }

    private IEnumerator MovementCoroutine(Animator animator, int parameterID, float end, float duration)
    {
        float elapsed = 0.0f;
        float start = animator.GetFloat(parameterID);

        do
        {
            animator.SetFloat(parameterID, Mathf.Lerp(start, end, blendCurve.Evaluate(elapsed / duration)));

            yield return null;
            elapsed += Time.deltaTime;
        }
        while (elapsed < duration);

        animator.SetFloat(parameterID, end);
    }
}
