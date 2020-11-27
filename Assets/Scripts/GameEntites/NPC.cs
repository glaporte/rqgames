﻿using System;
using System.Collections.Generic;
using DUCK.FSM;
using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC : MonoBehaviour, IPooledGameEntities
    {
        public const string NPCTag = "NPC";

        protected FiniteStateMachine<Playable.FSMCommon.State> _fsm;

        public Stack<GameObject> Container { get; set; }

        protected Quaternion InitialRotation { get; set; }
        protected Quaternion RotationOnStartMove { get; set; }

        protected float _internalTimer = 0;
        protected float _rndSmall;
        protected float _rndMedium;


        private void Start()
        {
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

            _fsm.Begin(Playable.FSMCommon.State.Idle);
            InitNPC();
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

        virtual protected void InitNPC()
        {

        }

        public void OnDie()
        {
            gameObject.SetActive(false);
            Container.Push(this.gameObject);
        }
    }

}