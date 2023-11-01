using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PixPlays.ElementalVFX
{
    public class ProjectileTester : MonoBehaviour
    {
        [SerializeField] GameObject _Prefab;
        [SerializeField] GameObject _CastPrefab;
        [SerializeField] GameObject _HitPrefab;
        [SerializeField] Transform _Source;
        [SerializeField] Transform _Target;
        [SerializeField] float _ProjectileSpeed;
        [SerializeField] AnimationCurve _HeightCurve;
        [SerializeField] float _MaxHeight;
        [SerializeField] float _ProjectileDelay;
        [SerializeField] float _DelayProjectileDestroy;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot(_Source.position,_Target.position, Vector3.zero);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Shoot(_Source.position,hit.point, hit.normal);
                }
            }
        }

        public void Shoot(Vector3 source,Vector3 target, Vector3 normal)
        {
            StartCoroutine(Coroutine_Projectile(source, target, normal));
        }

        IEnumerator Coroutine_Projectile(Vector3 sourcePos,Vector3 targetPos, Vector3 normal)
        {
            float lerp = 0;
            float distance = (sourcePos - targetPos).magnitude;
            if (_CastPrefab != null)
            {
                GameObject cast = Instantiate(_CastPrefab, sourcePos, Quaternion.identity);
                cast.transform.forward = (targetPos - _Source.position);
            }
            yield return new WaitForSeconds(_ProjectileDelay);
            GameObject projectile = Instantiate(_Prefab, sourcePos, Quaternion.identity);
            while (lerp < 1)
            {
                Vector3 pos = Vector3.Lerp(sourcePos, targetPos, lerp);
                pos.y += _HeightCurve.Evaluate(lerp) * _MaxHeight;
                projectile.transform.forward = (pos - projectile.transform.position);
                projectile.transform.position = pos;

                lerp += Time.deltaTime * _ProjectileSpeed / distance;
                yield return null;
            }
            if (_HitPrefab != null)
            {
                GameObject hit = Instantiate(_HitPrefab, projectile.transform.position, Quaternion.identity);
                if (normal == Vector3.zero)
                {
                    hit.transform.forward = (sourcePos - targetPos);
                }
                else
                {
                    hit.transform.forward = normal;
                }
            }
            Destroy(projectile.gameObject, _DelayProjectileDestroy);
        }
    }
}
