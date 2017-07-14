import socket
import time
import sys
import os
import thread
import numpy as np
import Leap
from Leap import Vector


class SampleListener(Leap.Listener):

    finger_names = ['thumb', 'index', 'middle', 'ring', 'pinky']
    bone_names   = ['metacarpal', 'proximal', 'intermediate', 'distal']
    state_names  = ['STATE_INVALID', 'STATE_START', 'STATE_UPDATE', 'STATE_END']

    def set_socket(self, socket, host):
        self.socket = socket
        self.host = host

    def on_init(self, controller):
        print "Initialized"
        self.throughput = 0
        self.prev_lframe = None
        self.prev_rframe = None
        self.pinch_confidence = 0

    def on_connect(self, controller):
        print "Motion sensor connected"
        # controller.enable_gesture(Leap.Gesture.TYPE_CIRCLE);
        # controller.enable_gesture(Leap.Gesture.TYPE_KEY_TAP);
        # controller.enable_gesture(Leap.Gesture.TYPE_SCREEN_TAP);
        # controller.enable_gesture(Leap.Gesture.TYPE_SWIPE);

    def on_disconnect(self, controller):
        print "Motion sensor disconnected"

    def on_exit(self, controller):
        print "Exited"

    def get_rotation_matrix(self, hand):
        basis = hand.basis
        sequence = basis.to_array_3x3()
        rotation = np.array([
            sequence[0:3],
            sequence[3:6],
            sequence[6:9]
        ])
        return rotation

    def get_rotation(self, hand, frame):
        prev_hand = self.prev_lframe if hand.is_left else self.prev_rframe
        if prev_hand is not None:
            rot_x = -hand.direction.pitch
            rot_y = hand.direction.yaw
            rot_z = hand.palm_normal.roll
            rx = np.rad2deg(rot_x)
            ry = np.rad2deg(rot_y)
            rz = np.rad2deg(rot_z)
            rotation = str(rx) + "," + str(ry) + "," + str(rz)
            return rotation
        return ""

    def get_pinch_state(self, hand):
        if hand.pinch_strength > 0.7:
            if self.pinch_confidence < self.throughput:
                self.pinch_confidence += 1
        else:
            if self.pinch_confidence > 0:
                self.pinch_confidence -= 1
        if self.pinch_confidence != 0:
            return "true"
        return "false"

    def get_position(self, hand, ibox):
        factor = 36.0
        point = hand.palm_position
        n = ibox.normalize_point(point)*factor
        p = [n.x-factor/2.0, n.y/2.0, -1*(n.z-factor/2.0)]
        return str(p[0])+","+str(p[1])+","+str(p[2])

    def on_frame(self, controller):
        frame = controller.frame()
        ibox = frame.interaction_box
        for hand in frame.hands:
            if hand.is_valid:
                htype = "left" if hand.is_left else "right"
                position = self.get_position(hand, ibox)
                rotation = self.get_rotation(hand, frame)
                pinch    = self.get_pinch_state(hand)
                tosend = htype+";"+position+";"+rotation+";"+pinch
                self.throughput += 1
                if self.throughput == 5:
                    self.throughput = 0
                    self.socket.sendto(tosend.encode(), self.host)
                if self.prev_lframe is None and hand.is_left:
                    self.prev_lframe = frame
                if self.prev_rframe is None and hand.is_right:
                    self.prev_rframe = frame

def run(host):
    server_address = (host, 9988)
    print "trying to create socket"
    client_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    listener = SampleListener()
    listener.set_socket(client_sock, server_address)
    controller = Leap.Controller()
    controller.add_listener(listener)
    print "press 'q' to quit OR 's[0-9]{2-3}' to select a scene"
    while True:
        entry = sys.stdin.readline()
        if entry.startswith('q'):
            break
        elif entry.startswith('s'):
            tosend = 'scene;' + entry[:-1]
            client_sock.sendto(tosend.encode(), server_address)
    controller.remove_listener(listener)




if __name__=="__main__":
    if len(sys.argv) != 2:
        print "usage: <this_program> <host_IP>"
        sys.exit()
    host = sys.argv[1]
    run(host)
