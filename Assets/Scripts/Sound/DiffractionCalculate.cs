using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Sound
{
    public class DiffractionCalculate : MonoBehaviour
    {
        [Header("Rays")]
        [SerializeField] private int _rayCount = 9;
        [SerializeField] private float _rayWidth = 1f;
        
        [Header("Layers")]
        [SerializeField] private LayerMask _PortalLayerMask;
        [SerializeField] private LayerMask _PlayerLayerMask;
        
        [Header("Diffraction Search")]
        [SerializeField] private float _portalSearchRadius = 30f;
        [SerializeField] private float _surfaceOffset = 0.05f;
        [SerializeField] private float _maxVirtualDistance = 100f;
        
        private Occlusion _occlusion;
        private StudioEventEmitter _mainEmitter;
        
        // Виртуальный инстанс FMOD
        private EventInstance _virtualInstance;
        private GameObject _virtualObject;
        private bool _virtualActive;

        // Лучший луч
        private RaycastHit _bestHit;
        private Vector3 _bestDirection;
        private float _bestRemainingDistance;
        private bool _hasBestHit;

        private void Start()
        {
            _occlusion = GetComponent<Occlusion>();
            _mainEmitter = GetComponent<StudioEventEmitter>();
            
            if (_mainEmitter == null)
            {
                Debug.LogError("На объекте нет StudioEventEmitter!", this);
            }
        }

        private void Update()
        {
            DrawDiffractionRays();
        }

        private void DrawDiffractionRays()
        {
            if (_occlusion.PlayerHead == null) return;
            if (_occlusion.CurrentOcclusion == 0) return;
            
            _bestRemainingDistance = float.PositiveInfinity;
            _hasBestHit = false;
            
            Vector3 center = _occlusion.PlayerHead.position;
            Vector3 directionToHead = (center - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, directionToHead).normalized;

            for (int i = 0; i < _rayCount; i++)
            {
                float t = (float)i / (_rayCount - 1);
                float offset = Mathf.Lerp(-_rayWidth * 0.5f, _rayWidth * 0.5f, t);
                Vector3 target = center + right * offset;
                
                Vector3 direction = target - transform.position;
                float fullDistance = direction.magnitude;

                if (ShootRay(direction, fullDistance, out RaycastHit hit))
                {
                    float remainingDistance = fullDistance - hit.distance;
                    
                    if (remainingDistance < _bestRemainingDistance)
                    {
                        _bestRemainingDistance = remainingDistance;
                        _bestHit = hit;
                        _bestDirection = direction;
                        _hasBestHit = true;
                    }
                }
            }
            
            if (_hasBestHit)
            {
                ProcessBestHit();
            }
            else
            {
                DisableVirtualSource();
            }
        }

        private bool ShootRay(Vector3 direction, float maxDistance, out RaycastHit hit)
        {
            if (Physics.Raycast(transform.position, direction.normalized, out hit, maxDistance, _PortalLayerMask))
            {
                Debug.DrawRay(transform.position, direction, Color.green);
                return true;
            }
            
            Debug.DrawRay(transform.position, direction, Color.gray);
            return false;
        }

        // ============================================
        // ОБРАБОТКА ЛУЧШЕГО ПОРТАЛА
        // ============================================
        private void ProcessBestHit()
        {
            Vector3 portalEdge = _bestHit.point + _bestHit.normal * _surfaceOffset;
            Vector3 playerPos = _occlusion.PlayerHead.position;
            
            if (CheckLineOfSight(portalEdge, playerPos, out _))
            {
                Debug.DrawLine(portalEdge, playerPos, Color.blue, Time.deltaTime, false);
                UpdateVirtualSource(portalEdge, playerPos, directPath: true);
            }
            else
            {
                Debug.DrawLine(portalEdge, playerPos, Color.yellow, Time.deltaTime, false);
                
                GameObject altPortal = FindAlternativePortal(portalEdge, playerPos);
                
                if (altPortal != null)
                {
                    Debug.DrawLine(portalEdge, altPortal.transform.position, Color.magenta, Time.deltaTime, false);
                    Debug.DrawLine(altPortal.transform.position, playerPos, Color.magenta, Time.deltaTime, false);
                    UpdateVirtualSource(altPortal.transform.position, playerPos, directPath: false);
                }
                else
                {
                    DisableVirtualSource();
                }
            }
        }

        // ============================================
        // ВИРТУАЛЬНЫЙ ИСТОЧНИК FMOD
        // ============================================
        private void UpdateVirtualSource(Vector3 portalPoint, Vector3 playerPos, bool directPath)
        {
            // 1. Расстояние от основного источника до портала
            float distSourceToPortal = Vector3.Distance(transform.position, portalPoint);
            
            // 2. Направление от игрока к порталу
            Vector3 playerToPortal = (portalPoint - playerPos).normalized;
            
            // 3. Позиция виртуального источника
            Vector3 virtualPos = portalPoint + playerToPortal * Mathf.Min(distSourceToPortal, _maxVirtualDistance);
            
            // Визуализация
            Debug.DrawLine(transform.position, portalPoint, Color.cyan, Time.deltaTime, false);
            Debug.DrawLine(portalPoint, virtualPos, Color.red, Time.deltaTime, false);
            Debug.DrawLine(virtualPos, playerPos, Color.white, Time.deltaTime, false);

            // Создаём или обновляем виртуальный инстанс
            EnsureVirtualInstance();
            
            // Обновляем 3D-позицию в FMOD
            FMOD.ATTRIBUTES_3D attributes = virtualPos.To3DAttributes();
            _virtualInstance.set3DAttributes(attributes);
            
            // Синхронизируем воспроизведение с основным эмиттером
            SyncVirtualPlayback();
            
            // Настраиваем громкость через параметр (если у тебя есть параметр "Volume" или "Diffraction")
            // Или через setVolume:
            float volume = directPath ? 0.8f : 0.4f;
            _virtualInstance.setVolume(volume);
        }

        private void EnsureVirtualInstance()
        {
            if (_virtualInstance.isValid()) return;
            if (_mainEmitter == null) return;

            // Создаём инстанс того же ивента, что и основной эмиттер
            _virtualInstance = RuntimeManager.CreateInstance(_mainEmitter.EventReference);
            
            // Создаём объект-контейнер для визуализации в редакторе (опционально)
            if (_virtualObject == null)
            {
                _virtualObject = new GameObject("VirtualFMODSource_" + gameObject.name);
            }
        }

        private void SyncVirtualPlayback()
        {
            if (!_virtualInstance.isValid()) return;
            
            // Проверяем, играет ли основной ивент
            _mainEmitter.EventInstance.getPlaybackState(out PLAYBACK_STATE mainState);
            _virtualInstance.getPlaybackState(out PLAYBACK_STATE virtualState);

            // Запускаем виртуальный, если основной играет, а виртуальный нет
            if (mainState == PLAYBACK_STATE.PLAYING && virtualState != PLAYBACK_STATE.PLAYING)
            {
                _virtualInstance.start();
            }
            // Останавливаем виртуальный, если основной остановлен
            else if (mainState != PLAYBACK_STATE.PLAYING && virtualState == PLAYBACK_STATE.PLAYING)
            {
                _virtualInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            
            // Синхронизируем timeline (защита от рассинхрона)
            if (mainState == PLAYBACK_STATE.PLAYING && virtualState == PLAYBACK_STATE.PLAYING)
            {
                _mainEmitter.EventInstance.getTimelinePosition(out int mainPos);
                _virtualInstance.getTimelinePosition(out int virtualPos);
                
                // Если разница больше 100 мс — подгоняем
                if (Mathf.Abs(virtualPos - mainPos) > 100)
                {
                    _virtualInstance.setTimelinePosition(mainPos);
                }
            }
        }

        private void DisableVirtualSource()
        {
            if (!_virtualInstance.isValid()) return;
            
            _virtualInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                _virtualInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        private void OnDestroy()
        {
            if (_virtualInstance.isValid())
            {
                _virtualInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _virtualInstance.release();
            }
            
            if (_virtualObject != null)
            {
                Destroy(_virtualObject);
            }
        }

        // ============================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ============================================
        private bool CheckLineOfSight(Vector3 from, Vector3 to, out RaycastHit hit)
        {
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            
            if (Physics.Raycast(from, dir.normalized, out hit, dist))
            {
                int hitLayer = hit.collider.gameObject.layer;
                return ((_PlayerLayerMask & (1 << hitLayer)) != 0);
            }
            
            return false;
        }

        private GameObject FindAlternativePortal(Vector3 from, Vector3 playerPos)
        {
            Collider[] portals = Physics.OverlapSphere(from, _portalSearchRadius, _PortalLayerMask);
            
            GameObject bestPortal = null;
            float bestScore = float.PositiveInfinity;

            foreach (Collider col in portals)
            {
                if (col == _bestHit.collider) continue;
                
                Vector3 portalPos = col.transform.position;
                
                if (IsBlocked(from, portalPos)) continue;
                
                Vector3 portalEdge = portalPos + Vector3.up * _surfaceOffset;
                if (!CheckLineOfSight(portalEdge, playerPos, out _)) continue;
                
                float totalDist = Vector3.Distance(from, portalPos) + Vector3.Distance(portalPos, playerPos);
                
                if (totalDist < bestScore)
                {
                    bestScore = totalDist;
                    bestPortal = col.gameObject;
                }
            }
            
            return bestPortal;
        }

        private bool IsBlocked(Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            
            if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, dist))
            {
                int hitLayer = hit.collider.gameObject.layer;
                bool isPortal = (_PortalLayerMask & (1 << hitLayer)) != 0;
                return !isPortal;
            }
            
            return false;
        }
    }
}