using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkenDD.Interfaces;

namespace ParkenDD.Services
{
    public class LifecycleService
    {
        private readonly List<ICanSuspend> _suspendObjects = new List<ICanSuspend>();
        private readonly List<ICanResume> _resumeObjects = new List<ICanResume>();

        public void Register(object obj)
        {
            var suspendObj = obj as ICanSuspend;
            if (suspendObj != null && !_suspendObjects.Contains(suspendObj))
            {
                _suspendObjects.Add(suspendObj);
            }
            var resumeObj = obj as ICanResume;
            if (resumeObj != null && !_resumeObjects.Contains(resumeObj))
            {
                _resumeObjects.Add(resumeObj);
            }
        }

        public Task SaveAsync()
        {
            return Task.Run(async () => await Task.WhenAll(_suspendObjects.Select(obj => obj.OnSuspend())));
        }

        public void Restore()
        {
            foreach (var obj in _resumeObjects)
            {
                obj.OnResume();
            }
        }
    }
}
