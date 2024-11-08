namespace Cleanup
{
    internal class Program
    {
        private const double TargetChangeTime = 1;

        private double _previousTargetSetTime;
        private bool _isTargetSet;
        private dynamic _lockedCandidateTarget;
        private dynamic _lockedTarget;
        private dynamic _target;
        private dynamic _previousTarget;
        private dynamic _activeTarget;
        private dynamic _targetInRangeContainer;

        public void CleanupTest(Frame frame)
        {
            try
            {
                if (_lockedCandidateTarget && !_lockedCandidateTarget.CanBeTarget)
                {
                    _lockedCandidateTarget = null;
                }

                if (_lockedTarget && !_lockedTarget.CanBeTarget)
                {
                    _lockedTarget = null;
                }

                _isTargetSet = false;
                // Sets _activeTarget field
                TrySetActiveTargetFromQuantum(frame);

                // If target exists and can be targeted, it should stay within Target Change Time since last target change
                if (_target && _target.CanBeTarget && Time.time - _previousTargetSetTime < TargetChangeTime)
                {
                    _isTargetSet = true;
                }
                _previousTarget = _target;

                if (!_isTargetSet)
                {
                    if (_lockedTarget && _lockedTarget.CanBeTarget)
                    {
                        _target = _lockedTarget;
                        _isTargetSet = true;
                        return;
                    }
                }


                if (!_isTargetSet)
                {
                    if (_activeTarget && _activeTarget.CanBeTarget)
                    {
                        _target = _activeTarget;
                        _isTargetSet = true;
                        return;
                    }
                }

                if (!_isTargetSet)
                {
                    _target = _targetInRangeContainer.GetTarget();
                    if (_target)
                    {
                        _isTargetSet = true;
                    }
                }
            }
            finally
            {
                if (_isTargetSet)
                {
                    if (_previousTarget != _target)
                    {
                        _previousTargetSetTime = Time.time;
                    }
                }
                else
                {
                    _target = null;
                }
                TargetableEntity.Selected = _target;
            }
        }

        // MORE CLASS CODE
    }
}
