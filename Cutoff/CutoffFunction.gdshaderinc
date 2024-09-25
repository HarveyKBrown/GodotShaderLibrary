float cutDistance(vec3 planeNormal, vec3 planePos, vec3 worldVertex) {
	float planeDistance = dot(planeNormal, planePos);
	float vertexDistance = dot(planeNormal, worldVertex);
	float dist = vertexDistance - planeDistance;

	return dist;
}
