﻿using System.Collections.Generic;
using DUCK.FSM;
using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC : MonoBehaviour, IPooledGameEntities
    {
        public const string NPCTag = "NPC";


        [SerializeField]
        protected rqgames.gameconfig.NPCConfig _config;

        protected FiniteStateMachine<Playable.FSMCommon.State> _fsm;

        protected Quaternion InitialRotation { get; set; }
        protected Quaternion RotationOnStartMove { get; set; }

        protected float _internalTimer = 0;
        protected float _rndSmall;
        protected float _rndMedium;

        protected float _weaponSpeed;
        protected float _countAttack;

        private float _timerAttack;

        private int _difficulty = 100; // if _countAttack is max (10), each second attack has _countAttack / _difficulty chance to proc.

        public Stack<GameObject> Container { get; set; }


        private void Start()
        {
            _timerAttack = 0;
            _rndMedium = UnityEngine.Random.Range(-15, 15);
            _rndSmall = UnityEngine.Random.Range(-1f, 1f);
            InitialRotation = transform.rotation;
            _fsm = FiniteStateMachine<Playable.FSMCommon.State>.FromEnum();
            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Attack, Playable.FSMCommon.ATTACK_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Move, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);

            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Move, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);

            _fsm.OnEnter(Playable.FSMCommon.State.Attack, Fire);

            _fsm.Begin(Playable.FSMCommon.State.Idle);
            InitNPC();
        }

        virtual protected void InitNPC() { }
        virtual protected void UpdateNPC() { }

        private void Update()
        {
            UpdateNPC();
            _timerAttack += Time.deltaTime;
            if (_timerAttack > 1)
            {
                if (UnityEngine.Random.Range(0, 1f) < _countAttack / _difficulty)
                    _fsm.IssueCommand(Playable.FSMCommon.ATTACK_COMMAND);
                _timerAttack = 0;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Init.GlobalVariables.AllyLayer
                && other.tag == Weapon.WeaponTag)
            {
                OnDie();
            }
        }

        virtual public void Rotate(float intensity)
        {
            transform.localRotation = RotationOnStartMove * Quaternion.Euler(0, 0, intensity * 30);
        }

        public void StartMove()
        {
            RotationOnStartMove = transform.rotation;
            _fsm.IssueCommand(Playable.FSMCommon.MOVE_COMMAND);
        }

        public void EndMove()
        {
            _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        virtual protected void Fire()
        {
            Vector3 vel = Init.PooledGameData.Player.transform.position - transform.position;
            vel = vel.normalized * _weaponSpeed;
            Vector3 pos = transform.position + transform.rotation * Vector3.back * transform.localScale.z;
            Init.PooledGameData.PopWeapon(pos,
                vel,
                Init.GlobalVariables.EnemyLayer);

            _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        public void OnDie()
        {
            gameObject.SetActive(false);
            Container.Push(this.gameObject);
        }
    }

}