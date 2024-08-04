# unity-grid3d

Simple implementation of a 3d grid. Converts to and from grid/world coordinates and does things like find clumping centered coordinates.

## How does it find next clump position?

It keeps track of a set of available positions, sorted by distance from the center. As things get added, the set is rearranged such that the next available position will clump it near the center.
