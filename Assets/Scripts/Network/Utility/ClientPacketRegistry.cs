public static class ClientPacketRegistry
{
    public static void RegisterAllHandlers(PacketDispatcher dispatcher)
    {
        var handlers = new AuthPacketHandler();
        var shopHandlers = new ShopPacketHandler();
        var rewardItemHandlers = new RewardItemPacketHandler();
        var equipHandlers = new EquipPacketHandler();
        var partyHandlers = new PartyPacketHandler();
        var characterHandlers = new CharacterPacketHandler();
        var worldMapHandlers = new WorldMapPacketHandler();

        dispatcher.RegisterHandler<LoginResponsePacket>(handlers.HandleLoginResponse);
        dispatcher.RegisterHandler<CharacterStatsResponsePacket>(handlers.HandleStatusResponse);

        dispatcher.RegisterHandler<WorldInfoResponsePacket>(worldMapHandlers.HandleWorldInfoResponsePacket);

        dispatcher.RegisterHandler<PurchaseItemResponsePacket>(shopHandlers.HandlePurchaseItemResponse);
        dispatcher.RegisterHandler<ShopItemListResponsePacket>(shopHandlers.HandleShopItemListResponse);

        dispatcher.RegisterHandler<GatheringResponsePacket>(rewardItemHandlers.HandleGatheringResponsePacket);

        dispatcher.RegisterHandler<WeaponItemEquipResponsePacket>(equipHandlers.HandleWeaponItemEquipResponse);
        dispatcher.RegisterHandler<EquipmentItemEquipResponsePacket>(equipHandlers.HandleEquipmentItemEquipResponse);
        dispatcher.RegisterHandler<EquipmentItemUnequipResponsePacket>(equipHandlers.HandleEquipmentItemUnequipResponse);
        dispatcher.RegisterHandler<EnhanceWeaponResponsePacket>(equipHandlers.HandleEnhanceWeaponItemResponse);
        dispatcher.RegisterHandler<EnhanceEquipmentResponsePacket>(equipHandlers.HandleEnhanceEquipmentItemResponse);

        dispatcher.RegisterHandler<UpdatePartyPresetResponsePacket>(partyHandlers.HandleUpdateDeployPartyPresetResponse);        

        dispatcher.RegisterHandler<UseCharacterExpItemResponse>(characterHandlers.HandleUseCharacterExpItemResponse);
        dispatcher.RegisterHandler<SwapActivePartyCharacterResponsePacket>(characterHandlers.HandleSwapActivePartyCharacterResponse);
        dispatcher.RegisterHandler<ReviveAllPartyMemberResponsePacket>(characterHandlers.HandleReviveAllPartyMemberResponsePacket);
        dispatcher.RegisterHandler<UseConsumeableItemResponsePacket>(characterHandlers.HandleUseConsumeableItemResponse);
    }
}