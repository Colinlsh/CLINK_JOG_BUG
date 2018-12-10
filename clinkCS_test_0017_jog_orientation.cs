//------------------------------------------------------------------------------
//
// ControLINK-API
// Copyright © Hanwha. All rights reserved.
//
// Seokhwan Kim  (seokhwan at hanwha.com)
// Kazuma Fukuda (k.fukuda at hanwha.com)
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace clinkv2api_test_cs
{
    public partial class clinkv2api_TestMain
    {
        public int runTest()
        {
            Console.WriteLine("===== (22) jog orientation =====");

            CLINK_API_RESULT caRetVal = CLINK_API_RESULT.CLINK_API_RESULT_OK;
            const float ROBOT_MOVE_VELOCITY = 50.0F;

            //---------------------------------------------------------------------
            // Safety limit 설정
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ Safety limit setting +++++++++");
            caRetVal = clinkCs.robot_safety_limit_tcp_speed_max_set(robotID, ROBOT_MOVE_VELOCITY);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_safety_limit_tcp_speed_max_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //caRetVal = clinkCs.robot_safety_limit_tcp_acc_max_set(robotID, ROBOT_MOVE_VELOCITY * 10);
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_safety_limit_tcp_acc_max_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}

            //---------------------------------------------------------------------
            // speed factor 설정
            //---------------------------------------------------------------------
            /*
            Console.WriteLine("+++++++++ Speed factor setting +++++++++");
            caRetVal = clinkCs.robot_speed_factor_set(robotID, 1.0F);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_speed_factor_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            Thread.Sleep(100);
            */

            //---------------------------------------------------------------------
            // servo on
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ servo on +++++++++");
            caRetVal = clinkCs.robot_servo_switch_set(robotID, CLINK_SWITCH.CLINK_SWITCH_ON);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_servo_switch_set(ON) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //---------------------------------------------------------------------
            //  각 축 homing
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ robot homing +++++++++");
            caRetVal = move_homing(robotID);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: move_homing() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 현재 로봇 위치
            float pos_x, pos_y, pos_z, ort_x, ort_y, ort_z;
            caRetVal = clinkCs.robot_tcp_pose_actual_get(robotID, out pos_x, out pos_y, out pos_z, out ort_x, out ort_y, out ort_z);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("---- info: (acutual) Position=({0}, {1}, {2}), Orientation=({3}, {4}, {5})", pos_x, pos_y, pos_z, ort_x, ort_y, ort_z);
            }
            else
            {
                Console.WriteLine("Error: clink_robot_tcp_pose_actual_get() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //---------------------------------------------------------------------
            // create a motion command data (TCP)
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ create motion command (TCP) +++++++++");
            uint baseMtCmdID = 0;
            caRetVal = clinkCs.motion_command_robot_tcp_create(out baseMtCmdID);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_tcp_create() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            caRetVal = clinkCs.motion_command_robot_robot_id_set(baseMtCmdID, robotID);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_robot_id_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //---------------------------------------------------------------------
            // 이동을 위한 parameter 설정
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ set motion parameters for TCP +++++++++");

            // MAX command speed
            caRetVal = clinkCs.motion_command_speed_max_set(baseMtCmdID, ROBOT_MOVE_VELOCITY);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_speed_max_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // MAX accelaration
            caRetVal = clinkCs.motion_command_acc_max_set(baseMtCmdID, ROBOT_MOVE_VELOCITY * 10);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_acc_max_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // MAX decelaration
            caRetVal = clinkCs.motion_command_dec_max_set(baseMtCmdID, ROBOT_MOVE_VELOCITY * 10);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_dec_max_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // Jerk percentage
            caRetVal = clinkCs.motion_command_jerk_percentage_set(baseMtCmdID, 1.0F);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_jerk_percentage_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // TCP coordination type
            caRetVal = clinkCs.motion_command_robot_tcp_coordination_set(baseMtCmdID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_tcp_coordination_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // TCP interpolator type
            caRetVal = clinkCs.motion_command_robot_tcp_interpolator_set(baseMtCmdID, CLINK_INTERPOLATOR.CLINK_INTERPOLATOR_LINEAR);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_tcp_interpolator_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇을 이동하는 위치 및 밤법을 설정
            caRetVal = clinkCs.motion_command_robot_tcp_position_end_set(baseMtCmdID, pos_x, pos_y, pos_z);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_tcp_position_end_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // TCP end orientation
            caRetVal = clinkCs.motion_command_robot_tcp_orientation_end_set(baseMtCmdID, ort_x, ort_y, ort_z);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_robot_tcp_orientation_end_set() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //---------------------------------------------------------------------
            //  Robot Orientation Jog Test (X/Y/Z 축 방향 만)
            //---------------------------------------------------------------------
            // 테스트 jog orientation base 이동명령 공통사항
            // Sleep() 로 일정 시간 이동 후 clink_stop()을 실행하여 멈춤
            // stop 함수 호출 후는 stop이 완료 될 때까지 event wait, target position 동기화를 시켜준다.
            // 그 후, 다른 robot orientation jog 명령 실행
            int eventResult;
            uint evWaitTime = 5000;
            const float ROBOT_JOG_MOVE_VELOCITY = 8.0F;
            const float jogDistance = 60.0F;        // distance

            ///////////////////////////////////////////////////
            //// ---  Z NEGATIVE JOG 2초 후 STOP (1)
            //Console.WriteLine("+++++++++ Z NEGATIVE JOG (1) +++++++++");
            //caRetVal = clinkCs.robot_jog_tcp_orientation_z(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_jog_tcp_orientation_z(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);    // sleep
            //caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}

            //// 로봇 이동이 완료될 때까지 기다린다.
            //caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            //if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    if (eventResult != 1)
            //    {
            //        Console.WriteLine(" ---- info: event not occurred: Z NEGATIVE JOG (1)");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);

            //// ---  Z POSITIVE JOG 2초 후 STOP
            //Console.WriteLine("+++++++++ Z POSITIVE JOG +++++++++");
            //caRetVal = clinkCs.robot_jog_tcp_orientation_z(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_POSITIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_jog_tcp_orientation_z(POSITIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);    // sleep
            //caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}

            //caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            //if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    if (eventResult != 1)
            //    {
            //        Console.WriteLine(" ---- info: event not occurred: Z POSITIVE JOG");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);

            //// ---  Z NEGATIVE JOG 2초 후 STOP (2)
            //Console.WriteLine("+++++++++ Z NEGATIVE JOG (2) +++++++++");
            //caRetVal = clinkCs.robot_jog_tcp_orientation_z(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_jog_tcp_orientation_z(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);    // sleep
            //caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            //if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}

            //// 로봇 이동이 완료될 때까지 기다린다.
            //caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            //if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            //{
            //    if (eventResult != 1)
            //    {
            //        Console.WriteLine(" ---- info: event not occurred: Z NEGATIVE JOG (2)");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            //}
            //Thread.Sleep(2000);

            /////////////////////////////////////////////////
            // ---  X NEGATIVE JOG ERROR OCCURED. ROBOT DID NOT ORIENTATE BUT TRANSLATE
            Console.WriteLine("+++++++++ X NEGATIVE JOG (1) +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_x(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_x(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇 이동이 완료될 때까지 기다린다.
            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: X NEGATIVE JOG (1)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            // ---  X POSITIVE JOG 2초 후 STOP
            Console.WriteLine("+++++++++ X POSITIVE JOG +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_x(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_POSITIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_x(POSITIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: X POSITIVE JOG");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            // ---  X NEGATIVE JOG ERROR OCCURED. ROBOT DID NOT ORIENTATE BUT TRANSLATE
            Console.WriteLine("+++++++++ X NEGATIVE JOG (2) +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_x(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_x(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇 이동이 완료될 때까지 기다린다.
            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: X NEGATIVE JOG (2)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            /////////////////////////////////////////////////
            // ---  Y POSITIVE JOG 시작 2초 후 STOP, 다시 2초 후 끝까지 이동
            Console.WriteLine("+++++++++ Y POSITIVE JOG +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_y(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_POSITIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_y(POSITIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇 이동이 완료될 때까지 기다린다.
            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Y POSITIVE JOG (1)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            Console.WriteLine(" ---- robot will move again within 2 sec ...");
            Thread.Sleep(2000);
            caRetVal = clinkCs.robot_jog_tcp_orientation_y(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_POSITIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_y(POSITIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Y POSITIVE JOG (2)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            // ---  Y NEGATIVE ERROR OCCURED. ROBOT DID NOT ORIENTATE BUT TRANSLATE
            Console.WriteLine("+++++++++ Y NEGATIVE JOG +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_y(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_y(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(3000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇 이동이 완료될 때까지 기다린다.
            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Y NEGATIVE JOG (1)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            Console.WriteLine(" ---- robot will move again within 2 sec ...");
            Thread.Sleep(2000);
            Console.WriteLine("+++++ Y Negative JOg ++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation_y(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, ROBOT_JOG_MOVE_VELOCITY, jogDistance);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation_y(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);    // sleep
            caRetVal = clinkCs.robot_stop(robotID, 0.5F);		// stop
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_stop() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // 로봇 이동이 완료될 때까지 기다린다.
            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_STOP_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Y NEGATIVE JOG (2)");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            //---------------------------------------------------------------------
            //  Robot Jog Test (임의의 방향)
            //---------------------------------------------------------------------
            evWaitTime = 30000;

            /////////////////////////////////////////////////
            // ---  임의의 방향 JOG 시작하고 500mm 이동한 후 STOP, 다시 2초 후 반대 방향으로 500mm 이동
            Console.WriteLine("+++++++++ Arbitrary direction JOG: base, moving distance = 500 mm +++++++++");
            caRetVal = clinkCs.robot_jog_tcp_orientation(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_POSITIVE, 15.0F, 30.0F, 45.0F, ROBOT_JOG_MOVE_VELOCITY * 2.0F, 50.0F);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation(POSITIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Arbitrary direction POSITIVE JOG");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }
            Thread.Sleep(2000);

            Console.WriteLine(" ---- robot will move again within 2 sec ...");
            Thread.Sleep(2000);
            caRetVal = clinkCs.robot_jog_tcp_orientation(robotID, CLINK_REF_COORDINATE.CLINK_REF_COORDINATE_BASE, CLINK_DIRECTION.CLINK_DIRECTION_NEGATIVE, 15.0F, 30.0F, 45.0F, ROBOT_JOG_MOVE_VELOCITY * 2.0F, 50.0F);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_jog_tcp_orientation(NEGATIVE) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            caRetVal = clinkCs.system_wait_event_group_subgroup((uint)CLINK_EVENT_GRP.CLINK_EVENT_GRP_MOTION_COMMAND, (uint)CLINK_EVENT_SUBGRP_MOTION_COMMAND.CLINK_EVENT_SUBGRP_MOTION_COMMAND_SETTLED_DOWN, evWaitTime, (char)1, out eventResult);
            if (caRetVal == CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                if (eventResult != 1)
                {
                    Console.WriteLine(" ---- info: event not occurred: Arbitrary direction NEGATIVE JOG");
                }
            }
            else
            {
                Console.WriteLine("Error: clink_system_wait_event_group_subgroup() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            // destroy motion command
            caRetVal = clinkCs.motion_command_destroy(baseMtCmdID);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_motion_command_destroy() ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            //---------------------------------------------------------------------
            // servo off
            //---------------------------------------------------------------------
            Console.WriteLine("+++++++++ servo off +++++++++");
            caRetVal = clinkCs.robot_servo_switch_set(robotID, CLINK_SWITCH.CLINK_SWITCH_OFF);
            if (caRetVal != CLINK_API_RESULT.CLINK_API_RESULT_OK)
            {
                Console.WriteLine("Error: clink_robot_servo_switch_set(OFF) ({0})", clinkCs.system_api_result_description_get(caRetVal));
            }

            return (int)caRetVal;
        }
    }
}
