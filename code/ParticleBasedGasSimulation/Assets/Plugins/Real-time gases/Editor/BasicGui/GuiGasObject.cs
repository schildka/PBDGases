using System;
using System.Collections.Generic;
using Common.LinearAlgebra;
using Common.Shapes;
using Plugins.Scripts.PositionBasedDynamics;
using Plugins.Scripts.PositionBasedDynamics.Area;
using UnityEditor;
using UnityEngine;

namespace Editor.BasicGui
{
    public class GuiGasObject : EditorWindow
    {
        public GameObject placeCube;
        public GameObject placeSphere;

        private static float _windowWidth = 1350;
        private static float _windowHeight = 470;

        private int _count = 0;
        private int _countRuntimeGas = 0;
        private int _countGas = 0;

        private bool _maxWind = false;
        private bool _maxGas = false;
        private bool _isLandscapeMode = true;

        private readonly List<bool> _isBox = new List<bool>();

        private readonly List<GameObject> _runtimeGas = new List<GameObject>();
        private readonly List<GameObject> _gas = new List<GameObject>();
        private readonly List<GameObject> _wind = new List<GameObject>();
        private readonly List<GameObject> _area = new List<GameObject>();

        private readonly List<Box3D> _windBox = new List<Box3D>();

        private readonly List<ParticleBounds> _particleBounds = new List<ParticleBounds>();

        private readonly List<Box3D> _gasBox = new List<Box3D>();

        private readonly List<Sphere3D> _gasSphere = new List<Sphere3D>();

        private readonly List<FluidCreation> _fluidCreation = new List<FluidCreation>();
        private readonly List<FluidCreation> _fluidRuntimeCreation = new List<FluidCreation>();

        private Vector2 _scrollPos;
        private Vector2 _scrollPosVertical;

        private void OnEnable()
        {
            _gasObject =
                (GameObject) AssetDatabase.LoadAssetAtPath(
                    "Assets/Plugins/Real-time gases/Prefabs/SphereParticle.prefab", typeof(GameObject));

            _areaObject =
                (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Plugins/Real-time gases/Prefabs/GasArea.prefab",
                    typeof(GameObject));

            placeCube = (GameObject) AssetDatabase.LoadAssetAtPath(
                "Assets/Plugins/Real-time gases/Prefabs/PlaceCube.prefab",
                typeof(GameObject));

            placeSphere =
                (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Plugins/Real-time gases/Prefabs/PlaceSphere.prefab",
                    typeof(GameObject));
        }

        [MenuItem("Real-time Gases/Create")]
        private static void Init()
        {
            var icon = (Texture2D) EditorGUIUtility.Load("d_Profiler.Physics");

            var windowPosX = (Screen.currentResolution.width / 1.1f) - _windowWidth;
            var windowPosY = (Screen.currentResolution.height / 2.0f) - _windowHeight;


            EditorWindow window = EditorWindow.GetWindow<GuiGasObject>();
            window.position = new Rect(windowPosX, windowPosY, _windowWidth, _windowHeight);

            window.titleContent = new GUIContent("Gas", icon);

            window.Show();
        }

        //Part1
        private static GameObject _gasObject;

        private readonly GUIContent _gasObjectToolTip = new GUIContent("Gas particle object",
            "Prefab of the gas object");

        private int _poolAmount = 10000;

        private readonly GUIContent _poolAmountToolTip = new GUIContent("Total amount of particles",
            "It's the number of gas particles which are pooled in an object pool");

        private float _timeStep = 1 / 15f;

        private readonly GUIContent _timeStepToolTip = new GUIContent("Time step in s.",
            "Time step of simulation. With decreasing number the simulation will become more accurate, but also slower.");

        private int _numberOfIterations = 1;

        private readonly GUIContent _numberOfIterationsToolTip = new GUIContent("Number of iterations",
            "Number determines how many iterations the fluid constraint has. With increasing number the simulation will become more accurate, but also slower.");

        private bool _simulationMode = true;

        private readonly GUIContent _simulationModeToolTip = new GUIContent("Simulation mode",
            "If checked, particles will only be simulated if e-key is pressed.");

        private double _simulationDistance = 100;

        private readonly GUIContent _simulationDistanceToolTip = new GUIContent("Simulation distance",
            "Particles are destroyed if distance to their origin is reached");

        private double _radius = 0.25;

        private readonly GUIContent _radiusToolTip = new GUIContent("Radius",
            "Radius of particles");

        private double _spawnRate = 0.5;

        private readonly GUIContent _runtimeTimeToolTip = new GUIContent(
            "Runtime spawn rate in s.",
            "After the stated time a new particle is created in runtime. This only affects the creation of runtime gas");

        private double _density = 1.98;

        private readonly GUIContent _densityToolTip = new GUIContent("Density in kg/m³",
            "Density of particles");

        private double _mass = 0.0;

        private readonly GUIContent _massToolTip = new GUIContent("Mass in kg",
            "Mass of particles. If mass is 0 it will be calculated with the formula: Mass = 0.8 * diameter³ * fluid.density");

        private double _damping = 0.025;

        private readonly GUIContent _dampingToolTip = new GUIContent("Damping in m/s",
            "Damping determines how the velocity of the Particles is slowed down over time");

        private double _viscosity = 0.01;

        private readonly GUIContent _viscosityToolTip = new GUIContent("Viscosity in Pa·s",
            "Viscosity determines how strong the Particles are bound together");

        private float _toxicity = 0.0f;

        private readonly GUIContent _toxicityToolTip = new GUIContent("Toxicity",
            "Toxicity determines how toxic one particle is. This has an effect on the densities of the areas.");

        //Part2

        private readonly List<string> _gasesRunText = new List<string>();

        private readonly List<Vector3> _gasesRunPosition = new List<Vector3>();

        private int _popUpStateGas = 0;
        private readonly string[] _popUpGasOptions = new string[] {"Cube", "Sphere"};

        private readonly GUIContent _popUpGasToolTip = new GUIContent("Shape of gas bounds",
            "These gases are placed in the scene right at the beginning");

        private readonly List<string> _gasesText = new List<string>();

        private readonly List<Vector3> _gasesPosition = new List<Vector3>();
        private readonly List<Vector3> _gasesScale = new List<Vector3>();


        //Part3

        private Vector3 _gravity = new Vector3(0, 0.81f, 0);

        private readonly GUIContent _gravityToolTip = new GUIContent("Gravity",
            "The gravitational force the particles are experiencing");

        private Vector3 _drag = new Vector3(0.1f, 0, 0);

        private readonly GUIContent _dragToolTip = new GUIContent("Drag",
            "The drag force the particles are experiencing");

        private readonly List<Vector3> _forces = new List<Vector3>();
        private readonly List<Vector3> _windPosition = new List<Vector3>();
        private readonly List<Vector3> _windScale = new List<Vector3>();

        //Part4
        private GameObject _areaObject;

        private readonly GUIContent _areaObjectToolTip = new GUIContent("Area object",
            "Prefab of the area object in which gas is calculated an visualized");

        private Color _emptyColor = new Color(0.39f, 0.39f, 0.39f, 0.59f);

        private readonly GUIContent _emptyColorToolTip = new GUIContent("Empty color",
            "Color of areas with few particles");

        private Color _denseColor = new Color(1f, 0f, 0f, 0.59f);

        private readonly GUIContent _denseColorToolTip = new GUIContent("Dense color",
            "Color of areas with many particles");

        private float _areaRadius = 10;

        private readonly GUIContent _areaRadiusToolTip = new GUIContent("Radius",
            "Radius of area(s)");

        private bool _staticArea = false;

        private readonly GUIContent _staticAreaToolTip = new GUIContent("Static Area",
            "If not checked, only one area exist, which is moving and scaling with the particles");

        private int _areaPoolAmount = 1;

        private readonly GUIContent _areaPoolAmountToolTip = new GUIContent("Total amount of areas",
            "It's the number of area objects which are pooled in an object pool");

        private bool _manuallyAreas = false;

        private readonly GUIContent _manuallyAreaToolTip = new GUIContent("Place manually",
            "If checked, gas is only calculated in the areas, that are placed in the world manually. Is not allowed to exceed amount of pool!");

        private readonly List<string> _manuallyAreaText = new List<string>();

        private readonly List<Vector3> _manuallyAreaPosition = new List<Vector3>();
        private readonly List<Vector3> _manuallyAreaScale = new List<Vector3>();

        // called on every window-update
        private void OnGUI()
        {
            _windowWidth = position.width;
            _windowHeight = position.height;

            _isLandscapeMode = _windowHeight <= _windowWidth;

            GUILayout.BeginArea(new Rect(10, 10, _windowWidth, _windowHeight));


            _scrollPos =
                EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width - 10),
                    GUILayout.Height(position.height - 10));


            if (_isLandscapeMode) GUILayout.BeginHorizontal(GUILayout.Height((_windowHeight / 2) - 25));
            else GUILayout.BeginVertical(GUILayout.Width((_windowWidth) - 25));

            GUILayout.BeginVertical(GUILayout.Width((_windowWidth / 4)));

            GUILayout.Label("Step.1 Define particle properties:", EditorStyles.boldLabel);

            _gasObject =
                (GameObject) EditorGUILayout.ObjectField(_gasObjectToolTip, _gasObject, typeof(GameObject), true);
            EditorGUILayout.Space();

            _poolAmount = EditorGUILayout.IntField(_poolAmountToolTip, _poolAmount);
            EditorGUILayout.Space();

            _timeStep = EditorGUILayout.Slider(_timeStepToolTip, _timeStep, 0.01f, 0.1f);
            EditorGUILayout.Space();

            _numberOfIterations = EditorGUILayout.IntSlider(_numberOfIterationsToolTip, _numberOfIterations, 1, 5);
            EditorGUILayout.Space();

            _simulationMode = EditorGUILayout.Toggle(_simulationModeToolTip, _simulationMode);
            EditorGUILayout.Space();

            _simulationDistance = EditorGUILayout.DoubleField(_simulationDistanceToolTip, _simulationDistance);
            EditorGUILayout.Space();

            _radius = EditorGUILayout.DoubleField(_radiusToolTip, _radius);
            EditorGUILayout.Space();

            _spawnRate = EditorGUILayout.DoubleField(_runtimeTimeToolTip, _spawnRate);
            EditorGUILayout.Space();

            GUILayout.Label(
                "The following attributes are saved when creating\ngas in step.2, thus they can differ from other gases",
                EditorStyles.label);

            _density = EditorGUILayout.DoubleField(_densityToolTip, _density);
            EditorGUILayout.Space();

            _mass = EditorGUILayout.DoubleField(_massToolTip, _mass);
            EditorGUILayout.Space();

            _damping = EditorGUILayout.DoubleField(_dampingToolTip, _damping);
            EditorGUILayout.Space();

            _viscosity = EditorGUILayout.DoubleField(_viscosityToolTip, _viscosity);
            EditorGUILayout.Space();

            _toxicity = EditorGUILayout.Slider(_toxicityToolTip, _toxicity, -1, 1);
            EditorGUILayout.Space();

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width((_windowWidth / 4)));

            GUILayout.Label("Step.2 Place gases in the scene:", EditorStyles.boldLabel);

            for (var i = 0; i < _gasesRunText.Count; i++)
            {
                _gasesRunPosition[i] = _runtimeGas[i].transform.position;

                _gasesRunText[i] = "Position: (" + _gasesRunPosition[i].x + "," + _gasesRunPosition[i].y +
                                   "," + _gasesRunPosition[i].z + ")";
                GUILayout.Label(_runtimeGas[i].name + " " + _gasesRunText[i], EditorStyles.label);

                _fluidRuntimeCreation[i].position =
                    new Vector3D(_gasesRunPosition[i].x, _gasesRunPosition[i].y, _gasesRunPosition[i].z);
            }

            if (GUILayout.Button("Create Runtime Gas"))
            {
                if ((_countRuntimeGas + _countGas) < 14)
                {
                    _runtimeGas.Add(Instantiate(placeSphere));
                    _runtimeGas[_countRuntimeGas].name = "RuntimeGas." + _countRuntimeGas++;

                    _gasesRunPosition.Add(new Vector3(0, 0, 0));
                    _gasesRunText.Add("");

                    _fluidRuntimeCreation.Add(
                        new FluidCreation(_runtimeGas[_runtimeGas.Count - 1], new Vector3D(0, 0, 0), _density, _mass,
                            _damping, _viscosity, _toxicity));
                }
                else
                {
                    _maxGas = true;
                }
            }

            EditorGUILayout.Space();


            for (var i = 0; i < _gasesText.Count; i++)
            {
                _gasesPosition[i] = _gas[i].transform.position;
                _gasesScale[i] = _gas[i].transform.lossyScale;

                if (_isBox[i])
                {
                    _gasBox[i] = _gasBox[i].SetBounds(_gasesPosition[i], _gasesScale[i]);

                    _particleBounds[i].bounds = _gasBox[i];
                    _particleBounds[i].spacing = _radius;

                    _particleBounds[i].CreateParticles();

                    _gasesText[i] = "Position: (" + Math.Round(_gasesPosition[i].x, 2) + "," +
                                    Math.Round(_gasesPosition[i].y, 2) + "," + Math.Round(_gasesPosition[i].z, 2) +
                                    ") Scale: (" + Math.Round(_gasesScale[i].x, 2) + "," +
                                    Math.Round(_gasesScale[i].y, 2) +
                                    "," + Math.Round(_gasesScale[i].z, 2) + ") Particles: " +
                                    _particleBounds[i].NumParticles;
                }
                else
                {
                    _gasSphere[i] = _gasSphere[i].SetBounds(_gasesPosition[i], _gasesScale[i].x / 2.0);

                    _particleBounds[i].sphereBounds = _gasSphere[i];
                    _particleBounds[i].spacing = _radius;

                    _particleBounds[i].CreateParticlesInSphere();

                    _gasesText[i] = "Position: (" + Math.Round(_gasesPosition[i].x, 2) + "," +
                                    Math.Round(_gasesPosition[i].y, 2) + "," + Math.Round(_gasesPosition[i].z, 2) +
                                    ") Radius: " + Math.Round(_gasesScale[i].x / 2.0, 2) + " Particles: " +
                                    _particleBounds[i].NumParticles;
                }

                _fluidCreation[i].bounds = _particleBounds[i];

                GUILayout.Label(_gas[i].name + " " + _gasesText[i], EditorStyles.label);
            }

            _popUpStateGas = EditorGUILayout.Popup(_popUpGasToolTip, _popUpStateGas, _popUpGasOptions);

            if (GUILayout.Button("Create Gas Bounds"))
            {
                if ((_countRuntimeGas + _countGas) < 14)
                {
                    if (_popUpStateGas.Equals(0))
                    {
                        _gas.Add(Instantiate(placeCube));
                        _gas[_countGas].name = "GasBox." + _countGas++;
                        _isBox.Add(true);
                    }
                    else
                    {
                        _gas.Add(Instantiate(placeSphere));
                        _gas[_countGas].name = "GasSphere." + _countGas++;
                        _isBox.Add(false);
                    }

                    _gasBox.Add(new Box3D());
                    _gasSphere.Add(new Sphere3D());

                    _gasesPosition.Add(new Vector3(0, 0, 0));
                    _gasesScale.Add(new Vector3(1, 1, 1));
                    _gasesText.Add("");
                    _particleBounds.Add(new ParticleBounds());
                    _fluidCreation.Add(
                        new FluidCreation(_particleBounds[_particleBounds.Count - 1], _density, _mass, _damping,
                            _viscosity, _toxicity));
                }
                else
                {
                    _maxGas = true;
                }
            }

            if (_maxGas) EditorGUILayout.HelpBox("Maximum reached", MessageType.Error);

            GUILayout.EndVertical();


            GUILayout.BeginVertical(GUILayout.Width((_windowWidth / 4)));


            GUILayout.Label("Step.3 Specify forces:",
                EditorStyles.boldLabel);

            _gravity = EditorGUILayout.Vector3Field(_gravityToolTip, _gravity);
            _drag = EditorGUILayout.Vector3Field(_dragToolTip, _drag);
            EditorGUILayout.Space();

            GUILayout.Label("Place wind areas with a specified force in the Scene",
                EditorStyles.label);

            for (var i = 0; i < _forces.Count; i++)
            {
                _forces[i] = EditorGUILayout.Vector3Field("Force." + i + " Position: (" +
                                                          Math.Round(_windPosition[i].x, 2) + "," +
                                                          Math.Round(_windPosition[i].y, 2) + "," +
                                                          Math.Round(_windPosition[i].z, 2) + ") Scale: (" +
                                                          Math.Round(_windScale[i].x, 2) + "," +
                                                          Math.Round(_windScale[i].y, 2) + "," +
                                                          Math.Round(_windScale[i].z, 2) + ")", _forces[i]);

                _windPosition[i] = _wind[i].transform.position;
                _windScale[i] = _wind[i].transform.lossyScale;
                _windBox[i] = _windBox[i].SetBounds(_windPosition[i], _windScale[i]);
            }

            if (GUILayout.Button("Create Wind Area"))
            {
                if (_count < 6)
                {
                    _forces.Add(new Vector3(0, 0, 0));
                    _windPosition.Add(new Vector3(0, 0, 0));
                    _windScale.Add(new Vector3(1, 1, 1));
                    _windBox.Add(new Box3D());

                    _wind.Add(Instantiate(placeCube));
                    _wind[_count].name = "Wind." + _count++;
                }
                else
                {
                    _maxWind = true;
                }
            }

            if (_maxWind) EditorGUILayout.HelpBox("Maximum reached", MessageType.Error);

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width((_windowWidth / 4) - 25));

            GUILayout.Label("Step.4 Define area properties and placement:", EditorStyles.boldLabel);

            _areaObject =
                (GameObject) EditorGUILayout.ObjectField(_areaObjectToolTip, _areaObject, typeof(GameObject), true);
            EditorGUILayout.Space();

            _emptyColor = EditorGUILayout.ColorField(_emptyColorToolTip, _emptyColor);
            EditorGUILayout.Space();

            _denseColor = EditorGUILayout.ColorField(_denseColorToolTip, _denseColor);
            EditorGUILayout.Space();

            _areaRadius = EditorGUILayout.FloatField(_areaRadiusToolTip, _areaRadius);
            EditorGUILayout.Space();

            _staticArea = EditorGUILayout.BeginToggleGroup(_staticAreaToolTip, _staticArea);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            _areaPoolAmount = EditorGUILayout.IntField(_areaPoolAmountToolTip, _areaPoolAmount);

            _manuallyAreas = EditorGUILayout.BeginToggleGroup(_manuallyAreaToolTip, _manuallyAreas);
            EditorGUI.indentLevel++;

            if (_isLandscapeMode)
                _scrollPosVertical =
                    EditorGUILayout.BeginScrollView(_scrollPosVertical, GUILayout.Height(position.height / 2 - 50));
            else
                _scrollPosVertical =
                    EditorGUILayout.BeginScrollView(_scrollPosVertical);

            for (var i = 0; i < _manuallyAreaText.Count; i++)
            {
                _manuallyAreaText[i] = "Position: (" + _manuallyAreaPosition[i].x + "," + _manuallyAreaPosition[i].y +
                                       "," + _manuallyAreaPosition[i].z + ")" + " Radius: " +
                                       (Math.Round(_manuallyAreaScale[i].x / 2, 2));
                GUILayout.Label(_area[i].name + " " + _manuallyAreaText[i], EditorStyles.label);

                _manuallyAreaPosition[i] = _area[i].transform.position;
                _manuallyAreaScale[i] = _area[i].transform.lossyScale;
            }

            if (GUILayout.Button("Create Area") && _area.Count < _areaPoolAmount)
            {
                _area.Add(Instantiate(placeSphere));
                _area[_area.Count - 1].name = "Area." + (_area.Count - 1);

                _manuallyAreaPosition.Add(new Vector3(0, 0, 0));
                _manuallyAreaScale.Add(new Vector3(0, 0, 0));
                _manuallyAreaText.Add("");
            }

            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndToggleGroup();

            GUILayout.EndVertical();

            if (_isLandscapeMode)
                GUILayout.EndHorizontal();
            else
                GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width((_windowWidth) - 25));

            EditorGUILayout.Space();

            VerifyValues();

            if (GUILayout.Button("Instantiate Gas Object"))
            {
                var gas = new GameObject("Gas");

                var pool = gas.AddComponent<GasPool>();
                var simulation = gas.AddComponent<GasSimulation>();
                var raster = gas.AddComponent<AreaRaster>();

                pool.gasToPool = _gasObject;
                pool.amountToPool = _poolAmount;


                simulation = AddForces(simulation);
                simulation.runtimeFluids = _fluidRuntimeCreation;
                simulation.fluids = _fluidCreation;
                simulation.timeStep = _timeStep;
                simulation.numberOfIterations = _numberOfIterations;
                simulation.simulationMode = _simulationMode;
                simulation.simulationDistance = _simulationDistance;
                simulation.radius = _radius;
                simulation.spawnRate = _spawnRate;


                raster.areaGasObject = _areaObject;
                raster.placedAreas = _area;
                raster.emptyColor = _emptyColor;
                raster.denseColor = _denseColor;
                raster.staticAreaMode = _staticArea;
                raster.placedAreaMode = _manuallyAreas;
                raster.areaRadius = _areaRadius;
                raster.amountOfAreaPool = _staticArea ? _areaPoolAmount : 1;
            }

            EditorGUILayout.Space();


            if (GUILayout.Button("Documentation"))
            {
                Debug.Log((Application.dataPath));
                Application.OpenURL((Application.dataPath) +
                                    "/Plugins/Real-time gases/Documentation/Documentation.pdf");
            }

            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private GasSimulation AddForces(GasSimulation simulation)
        {
            simulation.force = new Force
            {
                gravity = new Vector3D(_gravity.x, _gravity.y, _gravity.z),
                drag = new Vector3D(_drag.x, _drag.y, _drag.z),
                windForces = new List<Vector3D>(),
                windBoxes = new List<Box3D>()
            };

            foreach (var force in _forces)
            {
                simulation.force.windForces.Add(new Vector3D(force.x, force.y, force.z));
            }

            foreach (var windBox in _windBox)
            {
                simulation.force.windBoxes.Add(windBox);
            }

            return simulation;
        }

        private void VerifyValues()
        {
            if (_poolAmount < 1) _poolAmount = 1;
            if (_simulationDistance < 1) _simulationDistance = 1;
            if (_radius <= 0) _radius = 0.1;
            if (_spawnRate <= 0) _spawnRate = 0.1;
            if (_density <= 0) _density = 0.1;
            if (_mass < 0) _mass = 0;
            if (_damping <= 0) _damping = 0.025;
            if (_viscosity < 0) _viscosity = 0;
            if (_areaRadius <= 0) _areaRadius = 0.1f;
            if (_areaPoolAmount < 1) _areaPoolAmount = 1;
        }

        private void OnDestroy()
        {
            Debug.Log("Closing Window Event.");
        }
    }
}