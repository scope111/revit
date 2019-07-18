using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wxx_Revit
{
    class WxxRTU
    {
        /// <summary>
        /// Getting the specific material by name.
        /// </summary>
        /// <param name="name">The name of specific material.</param>
        /// <returns>The specific material</returns>
        public static Material GetMaterial(string name, UIDocument m_document)
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_document.Document);
            collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Materials));
            var MaterialElement = from element in collector
                                  where element.Name == name
                                  select element;

            if (MaterialElement.Count() == 0)
                return null;
            return MaterialElement.First<Element>() as Material;
        }

        /// <summary>
        /// Getting the specific material by name.
        /// </summary>
        /// <param name="name">The name of specific material.</param>
        /// <returns>The specific material</returns>
      /*  public static Material GetMaterialfromlibray(string name, Autodesk.Revit.ApplicationServices.Application revitApp)
        {
            FilteredElementCollector collector = new FilteredElementCollector(revitApp.Documents.);
            collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Materials));
            var MaterialElement = from element in collector
                                  where element.Name == name
                                  select element;

            if (MaterialElement.Count() == 0)
                return null;
            return MaterialElement.First<Element>() as Material;
        }*/
        /// <summary>
        /// Create a new brick material
        /// </summary>
        /// <returns>The specific material</returns>
        public static Material CreateSampleConcreteMaterial(UIDocument m_document)
        {
            Material materialNew = null;
            //Try to copy an existing material.  If it is not available, create a new one.
            Material masonry_Concrete = GetMaterial("Concrete, Lightweight", m_document);
            if (masonry_Concrete != null)
            {
                TaskDialog.Show("Tip", "找到这种材料了!");
                materialNew = masonry_Concrete.Duplicate(masonry_Concrete.Name + "_new");
                materialNew.MaterialClass = "Concrete";
            }
            else
            {
                ElementId idNew = Material.Create(m_document.Document, "New Concrete Sample");
                materialNew = m_document.Document.GetElement(idNew) as Material;
                materialNew.Color = new Autodesk.Revit.DB.Color(130, 150, 120);
            }

            //创建外观属性
          
            //Create a new structural asset and set properties on it.
            StructuralAsset structuralAsssetConcrete = new StructuralAsset("ConcreteStructuralAsset", Autodesk.Revit.DB.StructuralAssetClass.Concrete);
            structuralAsssetConcrete.ConcreteBendingReinforcement = .5;
            structuralAsssetConcrete.DampingRatio = .5;

            //Create a new thermal asset and set properties on it.
            ThermalAsset thermalAssetConcrete = new ThermalAsset("ConcreteThermalAsset", Autodesk.Revit.DB.ThermalMaterialType.Solid);
            thermalAssetConcrete.Porosity = 0.2;
            thermalAssetConcrete.Permeability = 0.3;
            thermalAssetConcrete.Compressibility = .5;
            thermalAssetConcrete.ThermalConductivity = .5;

            //Create PropertySets from assets and assign them to the material.
            PropertySetElement pseThermal = PropertySetElement.Create(m_document.Document, thermalAssetConcrete);
            PropertySetElement pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetConcrete);

            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pseStructural.Id);
            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, pseThermal.Id);

            return materialNew;
        }
        /// <summary>
        /// 更改外观
        /// </summary>
        /// <param name="material"></param>
        /// <param name="bumpmapImageFilepath"></param>
        public static void ChangeRenderingTexturePath(Material mat, Document doc, string texturePath, double x,double y)
        {

            using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
            {

                Asset editableAsset = editScope.Start(mat.AppearanceAssetId);

#if VERSION2018
                AssetProperty assetProperty = editableAsset["generic_diffuse"];
#else
                // VERSION2019
                AssetProperty assetProperty = editableAsset.FindByName("generic_diffuse");
#endif
                Asset connectedAsset = assetProperty.GetConnectedProperty(0) as Asset;
                /*if (connectedAsset == null)
                {

                    // Add a new default connected asset
                    assetProperty.AddConnectedAsset("UnifiedBitmap");
                    connectedAsset = assetProperty.GetSingleConnectedAsset();
                }*/
                if (connectedAsset.Name == "UnifiedBitmapSchema")
                {
                    // TaskDialog.Show("Tip", "成功");
                    AssetPropertyString path = connectedAsset.FindByName(UnifiedBitmap.UnifiedbitmapBitmap) as AssetPropertyString;

                    AssetPropertyBoolean scalelock = connectedAsset.FindByName(UnifiedBitmap.TextureScaleLock) as AssetPropertyBoolean;
                    scalelock.Value = false;
                    AssetPropertyDistance sizeY = connectedAsset.FindByName(UnifiedBitmap.TextureRealWorldScaleY) as AssetPropertyDistance;
                    sizeY.Value = y;
                    AssetPropertyDistance sizeX = connectedAsset.FindByName(UnifiedBitmap.TextureRealWorldScaleX) as AssetPropertyDistance;
                    sizeX.Value = x;
                    if (path.IsValidValue(texturePath))
                        path.Value = texturePath;
                    else
                    {
                        throw new Exception("找不到文件:" + texturePath);
                        //TaskDialog.Show("Tip", "文件路径错误");
                    }
                }

                editScope.Commit(true);

            }
        }
        /// <summary>
        /// 加载外观元素到文档中
        /// </summary>
        /// <param name="revitApp"></param>
        public static void loadAppearance(Autodesk.Revit.ApplicationServices.Application revitApp, Document doc,Material mat)
       {
       
            AssetSet theAssets = revitApp.get_Assets(AssetType.Appearance);
            foreach(Asset theAsset in theAssets)
            {
                // AppearanceAssetElement assetElem = AppearanceAssetElement.Create(doc, "haipi", theAsset);
      
               // FilteredElementCollector cotr = new FilteredElementCollector(doc);
          //     IEnumerable<AppearanceAssetElement> allApperar = cotr.OfClass(typeof(AppearanceAssetElement)).Cast<AppearanceAssetElement>();
               // mat.AppearanceAssetId = allApperar.First<AppearanceAssetElement>().Id;
            }
        }
        /// <summary>
        /// 获取材料信息
        /// </summary>
        /// <param name="material"></param>
        public static void GetMaterialInformation(Material material)
        {
            StringBuilder message = new StringBuilder("Material : " + material.Name);
            //color of the material
            message.Append(string.Format("\nColor: Red[{0}]; Green[{1}]; Blue[{2}]",
                            material.Color.Red, material.Color.Green, material.Color.Blue));
            //材料外观ID
            message.Append("id"+material.AppearanceAssetId);
            //foreground cut pattern and pattern color of the material
            FillPatternElement cutForegroundPattern = material.Document.GetElement(material.CutForegroundPatternId) as FillPatternElement;
            if (null != cutForegroundPattern)
            {
                message.Append("\nCut Foreground Pattern: " + cutForegroundPattern.Name);
                message.Append(string.Format("\nCut Foreground Pattern Color: Red[{0}]; Green[{1}]; Blue[{2}]",
                                material.CutForegroundPatternColor.Red, material.CutForegroundPatternColor.Green, material.CutForegroundPatternColor.Blue));
            }

            //foreground surface pattern and pattern color of the material
            FillPatternElement surfaceForegroundPattern = material.Document.GetElement(material.SurfaceForegroundPatternId) as FillPatternElement;
            if (null != surfaceForegroundPattern)
            {
                message.Append("\nSurface Foreground Pattern: " + surfaceForegroundPattern.Name);
                message.Append(string.Format("\nSurface Foreground Pattern Color: Red[{0}]; Green[{1}]; Blue[{2}]",
                                material.SurfaceForegroundPatternColor.Red, material.SurfaceForegroundPatternColor.Green, material.SurfaceForegroundPatternColor.Blue));
            }

            //background cut pattern and pattern color of the material
            FillPatternElement cutBackgroundPattern = material.Document.GetElement(material.CutBackgroundPatternId) as FillPatternElement;
            if (null != cutBackgroundPattern)
            {
                message.Append("\nCut Background Pattern: " + cutBackgroundPattern.Name);
                message.Append(string.Format("\nCut Background Pattern Color: Red[{0}]; Green[{1}]; Blue[{2}]",
                                material.CutBackgroundPatternColor.Red, material.CutBackgroundPatternColor.Green, material.CutBackgroundPatternColor.Blue));
            }

            //background surface pattern and pattern color of the material
            FillPatternElement surfaceBackgroundPattern = material.Document.GetElement(material.SurfaceBackgroundPatternId) as FillPatternElement;
            if (null != surfaceBackgroundPattern)
            {
                message.Append("\nSurface Background Pattern: " + surfaceBackgroundPattern.Name);
                message.Append(string.Format("\nSurface Background Pattern Color: Red[{0}]; Green[{1}]; Blue[{2}]",
                                material.SurfaceBackgroundPatternColor.Red, material.SurfaceBackgroundPatternColor.Green, material.SurfaceBackgroundPatternColor.Blue));
            }

            //some shading property of the material
            int shininess = material.Shininess;
            message.Append("\nShininess: " + shininess);
            int smoothness = material.Smoothness;
            message.Append("\nSmoothness: " + smoothness);
            int transparency = material.Transparency;
            message.Append("\nTransparency: " + transparency);

            TaskDialog.Show("Revit", message.ToString());
        }
        /// <summary>
        /// 设置墙材料
        /// </summary>
        /// <param name="wall"></param>
        public static void CreateCSforWall(Wall wall, UIDocument m_document,Material concrete)
        {
            // Get CompoundStructure


            WallType wallType = wall.WallType;
            //wallType.Name = wallType.Name + "_WithNewCompoundStructure";
            CompoundStructure wallCS = wallType.GetCompoundStructure();
            // Get material for CompoundStructureLayer

           // Material masonry_Brick = CreateSampleBrickMaterial();
            //Material concrete = CreateSampleConcreteMaterial(m_document);

            // Create CompoundStructureLayers and add the materials created above to them.
            List<CompoundStructureLayer> csLayers = new List<CompoundStructureLayer>();
            CompoundStructureLayer finish1Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish1, concrete.Id);
            //    CompoundStructureLayer substrateLayer = new CompoundStructureLayer(0.1, MaterialFunctionAssignment.Substrate, ElementId.InvalidElementId);
            // CompoundStructureLayer structureLayer = new CompoundStructureLayer(0.5, MaterialFunctionAssignment.Structure, concrete.Id);
            // CompoundStructureLayer membraneLayer = new CompoundStructureLayer(0, MaterialFunctionAssignment.Membrane, ElementId.InvalidElementId);
            // CompoundStructureLayer finish2Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish2, concrete.Id);
            csLayers.Add(finish1Layer);
            // csLayers.Add(substrateLayer);
            // csLayers.Add(structureLayer);
            //  csLayers.Add(membraneLayer);
            //csLayers.Add(finish2Layer);

            // Set the created layers to CompoundStructureLayer
            wallCS.SetLayers(csLayers);


            //Set which layer is used for structural analysis
            // wallCS.StructuralMaterialIndex = 2;

            // Set shell layers and wrapping.
            /* wallCS.SetNumberOfShellLayers(ShellLayerType.Interior, 2);
             wallCS.SetNumberOfShellLayers(ShellLayerType.Exterior, 1);
             wallCS.SetParticipatesInWrapping(0, false);

             // Points for adding wall sweep and reveal.
             UV sweepPoint = UV.Zero;
             UV revealPoint = UV.Zero;

             // split the region containing segment 0.
             int segId = wallCS.GetSegmentIds()[0];
             foreach (int regionId in wallCS.GetAdjacentRegions(segId))
             {
                 // Get the end points of segment 0.
                 UV endPoint1 = UV.Zero;
                 UV endPoint2 = UV.Zero;
                 wallCS.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

                 // Split a new region in split point and orientation.
                 RectangularGridSegmentOrientation splitOrientation = (RectangularGridSegmentOrientation)(((int)(wallCS.GetSegmentOrientation(segId)) + 1) % 2);
                 UV splitUV = (endPoint1 + endPoint2) / 2.0;
                 int newRegionId = wallCS.SplitRegion(splitUV, splitOrientation);
                 bool isValidRegionId = wallCS.IsValidRegionId(newRegionId);

                 // Find the enclosing region and the two segments intersected by a line through the split point
                 int segId1;
                 int segId2;
                 int findRegionId = wallCS.FindEnclosingRegionAndSegments(splitUV, splitOrientation, out segId1, out segId2);

                 // Get the end points of finding segment 1 and compute the wall sweep point.
                 UV eP1 = UV.Zero;
                 UV eP2 = UV.Zero;
                 wallCS.GetSegmentEndPoints(segId1, findRegionId, out eP1, out eP2);
                 sweepPoint = (eP1 + eP2) / 4.0;

                 // Get the end points of finding segment 2 and compute the wall reveal point.
                 UV ep3 = UV.Zero;
                 UV ep4 = UV.Zero;
                 wallCS.GetSegmentEndPoints(segId2, findRegionId, out ep3, out ep4);
                 revealPoint = (ep3 + ep4) / 2.0;
             }

             // Create a WallSweepInfo for wall sweep
             WallSweepInfo sweepInfo = new WallSweepInfo(true, WallSweepType.Sweep);
             PrepareWallSweepInfo(sweepInfo, sweepPoint.V);
             // Set sweep profile: Sill-Precast : 8" Wide
             sweepInfo.ProfileId = GetProfile("8\" Wide").Id;  
             sweepInfo.Id = 101;
             wallCS.AddWallSweep(sweepInfo);

             // Create a WallSweepInfo for wall reveal
             WallSweepInfo revealInfo = new WallSweepInfo(true, WallSweepType.Reveal);
             PrepareWallSweepInfo(revealInfo, revealPoint.U);
             revealInfo.Id = 102;
             wallCS.AddWallSweep(revealInfo);
             */
            // Set the new wall CompoundStructure to the type of wall.
            wallType.SetCompoundStructure(wallCS);
        }
        /// <summary>
        /// Custom filter for selection.
        /// </summary>
        private class IsPaintedFaceSelectionFilter : ISelectionFilter
        {
            Document selectedDocument = null;

            public bool AllowElement(Element element)
            {
                selectedDocument = element.Document;
                return true;
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                if (selectedDocument == null)
                {
                    throw new Exception("AllowElement was never called for this reference...");
                }

                Element element = selectedDocument.GetElement(refer);
               Face face = element.GetGeometryObjectFromReference(refer) as Face;

                return selectedDocument.IsPainted(element.Id, face);
            }
        }
        static void Log(string msg)
        {
            string dt = DateTime.Now.ToString("u");
            Trace.WriteLine(dt + " " + msg);
        }
        /// <summary>
        /// Get the painted material from selection.
        /// </summary>  
        public static Material GetPaintedMaterial(Document m_document, UIDocument uidoc)
        {
            
             Reference refer;
            Material m_currentMaterial;
            try
            {
                refer =uidoc.Selection.PickObject(ObjectType.Element);
                TaskDialog.Show("Tip", "获取到元素");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // Selection Cancelled.
                return null;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                Log(ex.Message);
                return null;
            }

            if (refer == null)
            {
                return null; 
            }
            else
            {
                FamilyInstance element = m_document.GetElement(refer) as FamilyInstance;
               
                //   Face face = element.GetGeometryObjectFromReference(refer) as Face;
                //  ElementId matId = m_document.GetPaintedMaterial(element.Id, face);
                //    m_currentMaterial = m_document.GetElement(matId) as Material;
                // m_currentMaterial = element.GetMaterialIds(true) as Material;
               // ElementId matId = element.GetMaterialIds(true) as ElementId;
               m_currentMaterial = m_document.GetElement(element.StructuralMaterialId) as Material;
                if (m_currentMaterial != null)
                {
                 //   m_currentAppearanceAssetElementId = m_currentMaterial.AppearanceAssetId;
                    TaskDialog.Show("Tip", "获取到材料");
                 
                    // Clear selection
                    uidoc.Selection.GetElementIds().Clear();
                }
                return m_currentMaterial;
            }

        }
        /// <summary>
        /// 创建球
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static DirectShape CreateSphereDirectShape(Document doc)
        {
            List<Curve> profile = new List<Curve>();

            // first create sphere with 2' radius
            XYZ center = XYZ.Zero;
            double radius = 50.0;
            XYZ profile00 = center;
            XYZ profilePlus = center + new XYZ(0, radius, 0);
            XYZ profileMinus = center - new XYZ(0, radius, 0);

            profile.Add(Line.CreateBound(profilePlus, profileMinus));
            profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

            CurveLoop curveLoop = CurveLoop.Create(profile);
            SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

            Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
            Solid sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
            DirectShape ds;
            using (Transaction t = new Transaction(doc, "Create sphere direct shape"))
            {
                t.Start();
                // create direct shape and assign the sphere shape
                ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                ds.SetShape(new GeometryObject[] { sphere });
                t.Commit();
            }
            return ds;
        }
        /// <summary>
        /// Create a DirectShape element from a BRepBuilder object and keep it in the _dbdocument.
        /// The main purpose is to display the BRepBuilder objects created.
        /// In this function, the BrepBuilder is directly set to a DirectShape.
        /// </summary>
        /// <param name="myBRepBuilder"> The BRepBuilder object.</param>
        /// <param name="name"> Name of the BRepBuilder object, which will be passed on to the DirectShape creation method.</param>
        private static void createDirectShapeElementFromBrepBuilderObject(BRepBuilder myBRepBuilder, String name, Document doc)
        {
            if (!myBRepBuilder.IsResultAvailable())
                return;

            using (Transaction tr = new Transaction( doc, "Create a DirectShape"))
            {
                tr.Start();

                DirectShape myDirectShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                myDirectShape.ApplicationId = "TestBRepBuilder";
                myDirectShape.ApplicationDataId = name;
               
                if (null != myDirectShape)
                    myDirectShape.SetShape(myBRepBuilder);
                tr.Commit();
            }
        }
        /// <summary>
        /// 创建圆柱
        /// </summary>
        public static void CreateCylinder(Document doc,Material mat)
        {
            // Naming convention for faces and edges: we assume that x is to the left and pointing down, y is horizontal and pointing to the right, z is up
            BRepBuilder brepBuilder = new BRepBuilder(BRepType.Solid);
           
            // The surfaces of the four faces.
            Frame basis = new Frame(new XYZ(5, -10, 0), new XYZ(0, 1, 0), new XYZ(-1, 0, 0), new XYZ(0, 0, 1));
            // Note that we do not have to create two identical surfaces here. The same surface can be used for multiple faces, 
            // since BRepBuilderSurfaceGeometry::Create() copies the input surface.
            // Thus, potentially we could have only one surface here, 
            // but we must create at least two faces below to account for periodicity. 
            CylindricalSurface frontCylSurf = CylindricalSurface.Create(basis, 5  );
            CylindricalSurface backCylSurf = CylindricalSurface.Create(basis, 5);
            Plane top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 100));  // normal points outside the cylinder
            Plane bottom = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 0)); // normal points inside the cylinder
                                                                                              // Note that the alternating of "inside/outside" matches the alternating of "true/false" in the next block that defines faces. 
                                                                                              // There must be a correspondence to ensure that all faces are correctly oriented to point out of the solid.

           // Add the four faces
            BRepBuilderGeometryId frontCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(frontCylSurf, null), false);
            BRepBuilderGeometryId backCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(backCylSurf, null), false);
            BRepBuilderGeometryId topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            BRepBuilderGeometryId bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);
            brepBuilder.SetFaceMaterialId(frontCylFaceId,mat.Id);
            brepBuilder.SetFaceMaterialId(backCylFaceId, mat.Id);
            // Geometry for the four semi-circular edges and two vertical linear edges
            BRepBuilderEdgeGeometry frontEdgeBottom = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -10, 0), new XYZ(10, -10, 0), new XYZ(5, -5, 0)));
            BRepBuilderEdgeGeometry backEdgeBottom = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(10, -10, 0), new XYZ(0, -10, 0), new XYZ(5, -15, 0)));

            BRepBuilderEdgeGeometry frontEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -10, 100), new XYZ(10, -10, 100), new XYZ(5, -5, 100)));
            BRepBuilderEdgeGeometry backEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -10, 100), new XYZ(10, -10, 100), new XYZ(5, -15, 100)));

            BRepBuilderEdgeGeometry linearEdgeFront = BRepBuilderEdgeGeometry.Create(new XYZ(10, -10, 0), new XYZ(10, -10, 100));
            BRepBuilderEdgeGeometry linearEdgeBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, -10, 0), new XYZ(0, -10, 100));

            // Add the six edges
            BRepBuilderGeometryId frontEdgeBottomId = brepBuilder.AddEdge(frontEdgeBottom);
            BRepBuilderGeometryId frontEdgeTopId = brepBuilder.AddEdge(frontEdgeTop);
            BRepBuilderGeometryId linearEdgeFrontId = brepBuilder.AddEdge(linearEdgeFront);
            BRepBuilderGeometryId linearEdgeBackId = brepBuilder.AddEdge(linearEdgeBack);
            BRepBuilderGeometryId backEdgeBottomId = brepBuilder.AddEdge(backEdgeBottom);
            BRepBuilderGeometryId backEdgeTopId = brepBuilder.AddEdge(backEdgeTop);

            // Loops of the four faces
            BRepBuilderGeometryId loopId_Top = brepBuilder.AddLoop(topFaceId);
            BRepBuilderGeometryId loopId_Bottom = brepBuilder.AddLoop(bottomFaceId);
            BRepBuilderGeometryId loopId_Front = brepBuilder.AddLoop(frontCylFaceId);
            BRepBuilderGeometryId loopId_Back = brepBuilder.AddLoop(backCylFaceId);

            // Add coedges for the loop of the front face
            brepBuilder.AddCoEdge(loopId_Front, linearEdgeBackId, false);
            brepBuilder.AddCoEdge(loopId_Front, frontEdgeTopId, false);
            brepBuilder.AddCoEdge(loopId_Front, linearEdgeFrontId, true);
            brepBuilder.AddCoEdge(loopId_Front, frontEdgeBottomId, true);
            brepBuilder.FinishLoop(loopId_Front);
            brepBuilder.FinishFace(frontCylFaceId);

            // Add coedges for the loop of the back face
            brepBuilder.AddCoEdge(loopId_Back, linearEdgeBackId, true);
            brepBuilder.AddCoEdge(loopId_Back, backEdgeBottomId, true);
            brepBuilder.AddCoEdge(loopId_Back, linearEdgeFrontId, false);
            brepBuilder.AddCoEdge(loopId_Back, backEdgeTopId, true);
            brepBuilder.FinishLoop(loopId_Back);
            brepBuilder.FinishFace(backCylFaceId);

            // Add coedges for the loop of the top face
            brepBuilder.AddCoEdge(loopId_Top, backEdgeTopId, false);
            brepBuilder.AddCoEdge(loopId_Top, frontEdgeTopId, true);
            brepBuilder.FinishLoop(loopId_Top);
            brepBuilder.FinishFace(topFaceId);

            // Add coedges for the loop of the bottom face
            brepBuilder.AddCoEdge(loopId_Bottom, frontEdgeBottomId, false);
            brepBuilder.AddCoEdge(loopId_Bottom, backEdgeBottomId, false);
            brepBuilder.FinishLoop(loopId_Bottom);
            brepBuilder.FinishFace(bottomFaceId);
          
            brepBuilder.Finish();

            createDirectShapeElementFromBrepBuilderObject(brepBuilder, "Full cylinder",doc);
        }
      
    }
}
